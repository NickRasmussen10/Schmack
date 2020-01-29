using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Temp UI Stuff
    /// </summary>
    [SerializeField] Slider flowSlider = null;

    [Header("Flow Testing Mode")]
    [SerializeField] bool flowTestMode = false;

    Rigidbody2D rb;

    [Header("Flow")]
    [SerializeField] float flowForgivenessTime = 1.0f;
    [SerializeField] float flowDepreciationRate = 1.0f;
    [SerializeField] float flowAppreciationRate = 1.0f;
    float flowJuice = 1.0f;
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
    [SerializeField] float stickTime = 1.0f; //how long the player stays in place for
    [SerializeField] float stickiness = 1.0f; //how slowly the player regains speed on wall stick
    [SerializeField] float horizontalForce = 50;
    [SerializeField] float wallJumpLimiter = 0.5f;

    float acceleration;
    float maxSpeed;
    float jumpForce;

    public Vector2 direction = Vector2.zero;
    Bow bow;

    RaycastHit2D[] raycastHits = new RaycastHit2D[5];
    Bounds playerBounds;

    bool isFalling = false;
    bool isGrounded = false;
    bool isWalking = false;
    bool isOnWall = false;

    bool limitHorizontalMovement = true;
    bool firstFrameOnWall = false;

    AudioManager audioMan;
    Animator anim_legs;
    Animator anim_arms;

    // Start is called before the first frame update
    void Start()
    {
        audioMan = AudioManager.instance;
        if (audioMan == null)
        {
            Debug.LogError("No audiomanager found");
        }
        anim_legs = GameObject.Find("legs").GetComponent<Animator>();
        anim_arms = GameObject.Find("arms").GetComponent<Animator>();
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

        HandleWallStick();

        if (!isOnWall)
        {
            CancelWallStick();
        }

        if (CheckRayCollision(0))
        {
            Bridge b = raycastHits[0].collider.gameObject.GetComponent<Bridge>();
            if (raycastHits[0].collider.gameObject.layer == 12 && b != null && b.direction != 0.0f)
            {
                transform.Translate(new Vector2(b.openingSpeed * b.direction * Time.deltaTime, 0.0f));
            }
            else
            {
                transform.parent = null;
            }
        }

        Animations();

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        //flow testing mode
        if (Input.GetButton("Jump") && Input.GetButton("Flow") && Input.GetButton("SwapWeapon")) flowTestMode = !flowTestMode;
    }

    void HandleFlow()
    {
        //if flow button is pressed and player is not in flow and player has a full flow bar
        if (Input.GetButtonDown("Flow") && !inFlow && flowJuice == 1.0f)
        {
            FlowChange();
        }
        else if(Input.GetButtonDown("Flow")  && inFlow)
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


        //flow testing mode
        if(flowTestMode && !inFlow)
        {
            FlowChange();
        }
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
            StartCoroutine(Flerp(true));
        }
        else
        {
            StartCoroutine(Flerp(false));
        }
    }

    IEnumerator Flerp(bool lerpUp)
    {
        float lerpVal;
        if (lerpUp) lerpVal = 0.0f;
        else lerpVal = 1.0f;

        CinemachineVirtualCamera vc = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

        while((lerpUp && lerpVal < 1.0f) || (!lerpUp && lerpVal > 0.0f))
        {
            if (lerpUp)
            {
                lerpVal += Time.deltaTime;
                if (lerpVal > 1.0f) lerpVal = 1.0f;
            }
            else
            {
                lerpVal -= Time.deltaTime * 4.0f;
                if (lerpVal < 0.0f) lerpVal = 0.0f;
            }

            acceleration = Mathf.Lerp(acceleration_slow, acceleration_fast, lerpVal);
            maxSpeed = Mathf.Lerp(maxSpeed_slow, maxSpeed_fast, lerpVal);
            jumpForce = Mathf.Lerp(jumpForce_slow, jumpForce_fast, lerpVal);

            vc.m_Lens.OrthographicSize = Mathf.Lerp(8.5f, 10.0f, lerpVal);
            
            yield return null;
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

        //if (Input.GetAxis("LeftHorizontal") < 0.05f && Input.GetAxis("LeftHorizontal") > -0.05f)
        //{
        //    if (isGrounded)
        //        isWalking = true;

        //    if (rb.velocity.x > 0 && (isGrounded || inFlow))
        //    {
        //        //apply friction to the left
        //        rb.AddForce(new Vector2(-acceleration, 0.0f));
        //        if (rb.velocity.x < 0)
        //        {
        //            rb.velocity = new Vector2(0, rb.velocity.y);
        //        }

        //    }
        //    else if (rb.velocity.x < 0 && (isGrounded || inFlow))
        //    {
        //        //apply friciton to the right
        //        rb.AddForce(new Vector2(acceleration, 0.0f));
        //        if (rb.velocity.x > 0)
        //        {
        //            rb.velocity = new Vector2(0, rb.velocity.y);
        //        }
        //    }
        //}
        //else
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
        if (isGrounded && !anim_legs.GetBool("onGround")) anim_legs.SetBool("onGround", true);
        else if (!isGrounded && anim_legs.GetBool("onGround")) anim_legs.SetBool("onGround", false);
        if (isGrounded && !anim_arms.GetBool("onGround")) anim_arms.SetBool("onGround", true);
        else if (!isGrounded && anim_arms.GetBool("onGround")) anim_arms.SetBool("onGround", false);

        anim_legs.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        anim_arms.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if (isOnWall && !anim_legs.GetBool("onWall")) anim_legs.SetBool("onWall", true);
        else if (!isOnWall && anim_legs.GetBool("onWall")) anim_legs.SetBool("onWall", false);
        if (isOnWall && !anim_arms.GetBool("onWall")) anim_arms.SetBool("onWall", true);
        else if (!isOnWall && anim_arms.GetBool("onWall")) anim_arms.SetBool("onWall", false);
    }

    /// <summary>
    /// handles detection and application of wall stick
    /// </summary>
    void HandleWallStick()
    {
        if (firstFrameOnWall)
        {
            StartCoroutine(WallStick());
        }
    }

    IEnumerator WallStick()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        acceleration /= 4;
        yield return new WaitForSeconds(stickTime);
        acceleration *= 4;
        while(rb.gravityScale < 1.0f)
        {
            rb.gravityScale += Time.deltaTime * stickiness;
            if (rb.gravityScale > 1.0f) rb.gravityScale = 1.0f;
            yield return null;
        }
    }

    void CancelWallStick()
    {
        StopCoroutine(WallStick());
        rb.gravityScale = 1.0f;
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
        Vector3 raycastStart = transform.position;
        raycastStart.y -= gameObject.GetComponent<CapsuleCollider2D>().bounds.size.y / 2;
        if (((raycastHits[1] = Physics2D.Raycast(raycastStart, Vector2.left, (playerBounds.size.x / 2) + 0.2f, finalLayerMask)).collider != null ||
            (raycastHits[2] = Physics2D.Raycast(raycastStart, Vector2.right, (playerBounds.size.x / 2) + 0.2f, finalLayerMask)).collider != null ||
            (raycastHits[3] = Physics2D.Raycast(transform.position, Vector2.left, (playerBounds.size.x / 2) + 0.2f, finalLayerMask)).collider != null ||
            (raycastHits[4] = Physics2D.Raycast(transform.position, Vector2.right, (playerBounds.size.x / 2) + 0.2f, finalLayerMask)).collider != null) &&
            !isGrounded)
        {
            if (isOnWall) firstFrameOnWall = false;
            else firstFrameOnWall = true;
            isOnWall = true;
        }
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
