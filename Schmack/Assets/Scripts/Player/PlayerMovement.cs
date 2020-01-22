﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Temp UI Stuff
    /// </summary>
    [SerializeField] Slider flowSlider = null;

    Rigidbody2D rb;

    [Header("Flow")]
    [SerializeField] float flowForgivenessTime = 1.0f;
    [SerializeField] float flowDepreciationRate = 1.0f;
    [SerializeField] float flowAppreciationRate = 1.0f;
    float flowJuice = 0.0f;
    public bool inFlow = false;
    float flowTimer = 0.0f;
    float flowThreshold = 0.0f;


    [Header("Movement - Flow")]
    [SerializeField] float acceleration_fast = 8;
    [SerializeField] float maxSpeed_fast = 10;
    [SerializeField] float jumpForce_fast = 250;

    [Header("Movement - No Flow")]
    [SerializeField] float acceleration_slow = 4;
    [SerializeField] float maxSpeed_slow = 5;
    [SerializeField] float jumpForce_slow = 200;

    [Header("Wall sticking")]
    [SerializeField] float stickiness = 1.0f;
    [SerializeField] float horizontalForce = 50;
    [SerializeField] float wallJumpLimiter = 0.5f;

    float acceleration;
    float maxSpeed;
    float jumpForce;

    public Vector2 direction = Vector2.zero;
    Bow bow;

    RaycastHit2D[] raycastHits = new RaycastHit2D[3];
    Bounds playerBounds;

    bool isFalling = false;
    bool isGrounded = false;
    bool isWalking = false;
    bool isOnWall = false;

    bool limitHorizontalMovement = true;


    private Animator anim;


    AudioManager audioMan;

    // Start is called before the first frame update
    void Start()
    {
        audioMan = AudioManager.instance;
        if (audioMan == null)
        {
            Debug.LogError("No audiomanager found");
        }
        anim = GetComponentInChildren<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        bow = gameObject.GetComponentInChildren<Bow>();
        inFlow = false;
        acceleration = acceleration_slow;
        maxSpeed = maxSpeed_slow;
        jumpForce = jumpForce_slow;
        flowThreshold = maxSpeed_slow * maxSpeed_slow * 0.5f;
        direction = new Vector2(1.0f, 1.0f);
        playerBounds = gameObject.GetComponent<CapsuleCollider2D>().bounds;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            audioMan.PlaySound("Walk");
        }
        HandleFlow();
        JoystickMovement();
        UpdatePlayerDirection();
        CastRays();


        //if player is going downward, flag them as falling
        isFalling = rb.velocity.y < 0 ? true : false;
        Jump();
        WallStick();
        Animations();

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }

    void HandleFlow()
    {
        //if flow button is pressed and player is not in flow and player has a full flow bar
        if (Input.GetButtonDown("Flow") && !inFlow && flowJuice == 1.0f)
        {
            FlowChange();
        }

        //if player is in flow and it not moving fast enough
        if (inFlow && rb.velocity.sqrMagnitude < flowThreshold)
        {
            StartCoroutine("TurtleTime");
        }

        //if player is in flow and moving fast enough
        if (rb.velocity.sqrMagnitude >= flowThreshold && flowJuice < 1.0f)
        {
            StopCoroutine("TurtleTime");
            flowTimer = flowForgivenessTime;
            flowJuice += Time.deltaTime * flowAppreciationRate;
            if (flowJuice > 1.0f) flowJuice = 1.0f;
        }
        //if flow forgiveness timer is up
        else if (flowTimer == 0 && flowJuice > 0.0f && inFlow)
        {
            flowJuice -= Time.deltaTime * flowDepreciationRate;
            if (flowJuice < 0.0f) flowJuice = 0.0f;
        }



        if (inFlow && flowJuice == 0)
        {
            FlowChange();
        }

        flowSlider.value = flowJuice;
    }

    IEnumerator TurtleTime()
    {
        while (flowTimer > 0)
        {
            flowTimer -= Time.deltaTime;
            if (flowTimer < 0) flowTimer = 0;
            yield return null;
        }
    }



    /// <summary>
    /// handles swapping between vibe state and yuck state
    /// </summary>
    void FlowChange()
    {
        inFlow = !inFlow;

        if (inFlow)
        {
            acceleration = acceleration_fast;
            maxSpeed = maxSpeed_fast;
            jumpForce = jumpForce_fast;
        }
        else
        {
            acceleration = acceleration_slow;
            maxSpeed = maxSpeed_slow;
            jumpForce = jumpForce_slow;
        }
    }

    void JoystickMovement()
    {
        if (limitHorizontalMovement)
        {
            float hInput = Input.GetAxis("LeftHorizontal");
            rb.AddForce(new Vector2(hInput * acceleration, 0.0f));
        }
        else
        {
            float hInput = Input.GetAxis("LeftHorizontal");
            rb.AddForce(new Vector2(hInput * acceleration / 4, 0.0f));
        }

            if (Input.GetAxis("LeftHorizontal") < 0.05f && Input.GetAxis("LeftHorizontal") > -0.05f)
            {
                if (isGrounded)
                    isWalking = true;

                if (rb.velocity.x > 0 && isGrounded)
                {
                    //apply friction to the left
                    rb.AddForce(new Vector2(-acceleration, 0.0f));
                    if (rb.velocity.x < 0)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }

                }
                else if (rb.velocity.x < 0 && isGrounded)
                {
                    //apply friciton to the right
                    rb.AddForce(new Vector2(acceleration, 0.0f));
                    if (rb.velocity.x > 0)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
            }
            else
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
            if (rb.velocity.x > 0)
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            else if (rb.velocity.x < 0)
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
        
    }

    IEnumerator DisableHorizontalMovement()
    {
        limitHorizontalMovement = false;
        yield return new WaitForSeconds(0.75f);
        limitHorizontalMovement = true;
        
    }

    void UpdatePlayerDirection()
    {
        float x;
        if (isOnWall)
        {
            direction.x = CheckRayCollision(1) ? 1 : -1;

        }
        else if (Mathf.Abs(Input.GetAxis("RightHorizontal")) > 0)
        {
            x = Input.GetAxis("RightHorizontal");
            direction.x = x > 0 ? 1 : -1;
        }
        else if (Mathf.Abs(Input.GetAxis("LeftHorizontal")) > 0)
        {
            x = Input.GetAxis("LeftHorizontal");
            direction.x = x > 0 ? 1 : -1;
        }

        gameObject.transform.localScale = direction;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0.0f, jumpForce));
            }
            else if (CheckRayCollision(1))
            {
                rb.AddForce(new Vector2(horizontalForce, jumpForce / wallJumpLimiter));
                StartCoroutine(DisableHorizontalMovement());
            }
            else if (CheckRayCollision(2))
            {
                rb.AddForce(new Vector2(-horizontalForce, jumpForce / wallJumpLimiter));
                StartCoroutine(DisableHorizontalMovement());
            }
        }
    }


    void Animations()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("OnWall", isOnWall);
        anim.SetBool("IsJumping", !isGrounded && !isOnWall);
        anim.SetBool("IsVibing", inFlow);
        anim.SetBool("Drawn", bow.isDrawnBack);
        anim.SetBool("Fire", bow.fire);
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && anim.GetCurrentAnimatorStateInfo(0).IsName("standing_fire"))
        {
            anim.Play("standing_load");
        }
    }

    /// <summary>
    /// handles detection and application of wall stick
    /// </summary>
    void WallStick()
    {
        if (isOnWall && isFalling)
        {
            rb.AddForce(-Physics2D.gravity / stickiness);
        }
    }

    /// <summary>
    /// casts rays from player's center
    /// </summary>
    void CastRays()
    {
        int layer_environment = 1 << 9;
        int layer_enemies = 1 << 11;
        int layer_interactables = 1 << 12;
        int finalLayerMask = layer_environment | layer_enemies | layer_interactables;
        //cast a ray downward, and if it hits the environment or an enemy set isGrounded = true
        if ((raycastHits[0] = Physics2D.Raycast(transform.position, Vector2.down, (playerBounds.size.y / 2) + 0.1f, finalLayerMask)).collider != null)
            isGrounded = true;
        else
            isGrounded = false;

        //cast a ray left and right, if either hits and player is not on ground, set isOnWall = true
        if (((raycastHits[1] = Physics2D.Raycast(transform.position, Vector2.left, (playerBounds.size.x / 2) + 0.2f, finalLayerMask)).collider != null ||
            (raycastHits[2] = Physics2D.Raycast(transform.position, Vector2.right, (playerBounds.size.x / 2) + 0.2f, finalLayerMask)).collider != null) &&
            !isGrounded)
            isOnWall = true;
        else
            isOnWall = false;
    }

    /// <summary>
    /// returns true if the ray at the specified index has a collider that is not the player (i.e. player is colliding on that side)
    /// </summary>
    /// <param name="index">index of the raycastHit to test from raycastHits array</param>
    /// <returns></returns>
    bool CheckRayCollision(int index)
    {
        return raycastHits[index].collider != null;
    }

    /// <summary>
    /// applies the given force to the player
    /// </summary>
    /// <param name="knockback">the force to be applied to the player</param>
    /// <param name="groundRequired">does the player need to be on the ground?</param>
    public void AddKnockback(Vector2 knockback, bool groundRequired)
    {
        Debug.Log(knockback);
        rb.velocity = Vector2.zero;
        if (groundRequired)
        {
            if (!CheckRayCollision(0))
            {
                rb.AddForce(knockback);
            }
        }
        else
        {
            rb.AddForce(knockback);
        }

    }

    /// <summary>
    /// helper function to RigidBody.AddForce to allow seperated components without having to manually make a new vector
    /// </summary>
    /// <param name="x">force in the x direction</param>
    /// <param name="y">force in the y direction</param>
    private void AddForce(float x, float y) { rb.AddForce(new Vector2(x, y)); }
}
