﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Vibe")]
    [SerializeField] float timeToVibe = 3;

    [Header("Movement - Vibing")]
    [SerializeField] float acceleration_fast = 8;
    [SerializeField] float maxSpeed_fast = 10;
    [SerializeField] float jumpForce_fast = 250;

    [Header("Movement - Not Vibing")]
    [SerializeField] float acceleration_slow = 4;
    [SerializeField] float maxSpeed_slow = 5;
    [SerializeField] float jumpForce_slow = 200;

    [Header("Wall sticking")]
    [SerializeField] float stickiness = 1.0f;
    [SerializeField] float horizontalForce = 50;
    [SerializeField] float wallJumpLimiter = 1.5f;

    float acceleration;
    float maxSpeed;
    float jumpForce;
    float vibeThreshold = 0.0f;

    RaycastHit2D[] raycastHits = new RaycastHit2D[3];

    bool isFalling = false;
    bool vibing = false;
    public bool GetVibing() { return vibing; }

    float vibeTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        vibing = false;
        acceleration = acceleration_slow;
        maxSpeed = maxSpeed_slow;
        jumpForce = jumpForce_slow;
        vibeThreshold = maxSpeed_slow * maxSpeed_slow * 0.9f;
        vibeTimer = timeToVibe;
    }

    // Update is called once per frame
    void Update()
    {
        VibeCheck();
        if (vibeTimer <= 0)
        {
            VibeChange();
        }
        JoystickMovement();
        CastRays();
        //if player is going downward, flag them as falling
        isFalling = rb.velocity.y < 0 ? true : false;
        Jump();
        WallStick();
    }

    void VibeCheck()
    {
        if((!vibing && rb.velocity.sqrMagnitude > vibeThreshold) || (vibing && rb.velocity.sqrMagnitude < vibeThreshold / 2))
        {
            Debug.Log("vibe incoming");
            vibeTimer -= Time.deltaTime;
        }
        else
        {
            vibeTimer = timeToVibe;
        }
    }



    /// <summary>
    /// handles swapping between vibe state and yuck state
    /// </summary>
    void VibeChange()
    {
        Debug.Log("vibe check");
        vibing = !vibing;

        if (vibing)
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
        rb.AddForce(new Vector2(Input.GetAxis("LeftHorizontal") * acceleration, 0.0f));
        if (Input.GetAxis("LeftHorizontal") < 0.05f && Input.GetAxis("LeftHorizontal") > -0.05f)
        {
            if (rb.velocity.x > 0 && CheckRayCollision(0))
            {
                //apply friction to the left

                rb.AddForce(new Vector2(-acceleration, 0.0f));
                if (rb.velocity.x < 0)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            else if (rb.velocity.x < 0 && CheckRayCollision(0))
            {
                //apply friciton to the right

                rb.AddForce(new Vector2(acceleration, 0.0f));
                if (rb.velocity.x > 0)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (CheckRayCollision(0))
            {
                rb.AddForce(new Vector2(0.0f, jumpForce));
            }
            else if (CheckRayCollision(1))
            {
                rb.AddForce(new Vector2(horizontalForce, jumpForce / wallJumpLimiter));
            }
            else if (CheckRayCollision(2))
            {
                rb.AddForce(new Vector2(-horizontalForce, jumpForce / wallJumpLimiter));
            }
        }
    }

    /// <summary>
    /// handles detection and application of wall stick
    /// </summary>
    void WallStick()
    {
        if ((CheckRayCollision(1) || CheckRayCollision(2)) && isFalling)
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
        int finalLayerMask = layer_environment | layer_enemies;
        raycastHits[0] = Physics2D.Raycast(transform.position, Vector2.down, (gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2) + 0.1f, finalLayerMask);
        raycastHits[1] = Physics2D.Raycast(transform.position, Vector2.left, (gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2) + 0.1f, finalLayerMask);
        raycastHits[2] = Physics2D.Raycast(transform.position, Vector2.right, (gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2) + 0.1f, finalLayerMask);
    }

    /// <summary>
    /// returns true if the ray at the specified index has a collider that is not the player (i.e. player is colliding on that side)
    /// </summary>
    /// <param name="index">index of the raycastHit to test from raycastHits array</param>
    /// <returns></returns>
    bool CheckRayCollision(int index)
    {
        //oof ouch owie my code
        return raycastHits[index].collider != null;
    }

    /// <summary>
    /// applies the given force to the player
    /// </summary>
    /// <param name="knockback">the force to be applied to the player</param>
    /// <param name="groundRequired">does the player need to be on the ground?</param>
    public void AddKnockback(Vector2 knockback, bool groundRequired)
    {
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
