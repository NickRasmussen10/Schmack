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
    //[SerializeField] float stickiness = 1.0f; //how slowly the player regains speed on wall stick
    [SerializeField] float horizontalForce = 50;
    [SerializeField] float wallJumpLimiter = 0.5f;


    enum PlayerState
    {
        idle,
        running,
        jumping,
        wallSticking,
        ledgeGrabbing
    }

    PlayerState state;

    float acceleration;
    float maxSpeed;
    float jumpForce;

    public Vector2 direction = Vector2.zero;
    Bow bow;

    CollisionPacket collPacket_ground;
    CollisionPacket collPacket_frontLegs;
    CollisionPacket collPacket_backLegs;
    CollisionPacket collPacket_frontTorso;

    bool isFalling = false;
    //bool isGrounded = false;
    //bool isWalking = false;
    bool isOnWall = false;

    float movementLimiter = 1.0f;
    bool wallStickIsRunning = false;


    Animator anim_legs;
    Animator anim_arms;
    Animator anim_spine;

    // Start is called before the first frame update
    void Start()
    {
        anim_legs = GameObject.Find("legs").GetComponent<Animator>();
        anim_arms = GameObject.Find("arms").GetComponent<Animator>();
        anim_spine = GameObject.Find("spine").GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        bow = gameObject.GetComponentInChildren<Bow>();
        inFlow = false;
        state = PlayerState.idle;
        acceleration = acceleration_slow;
        maxSpeed = maxSpeed_slow;
        jumpForce = jumpForce_slow;
        flowThreshold = maxSpeed_slow * maxSpeed_slow * 0.5f;
        direction = new Vector2(1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleFlow();
        JoystickMovement();
        UpdatePlayerDirection();


        //if player is going downward, flag them as falling
        isFalling = rb.velocity.y < 0 ? true : false;
        Jump();

        HandleWallStick();

        SetPlayerState();

        if (state != PlayerState.wallSticking && state != PlayerState.ledgeGrabbing)
        {
            CancelWallStick();
        }

        ///TODO: this moves the player with the bridge, make it a seperate script preferably on bridge
        //if (collPacket_ground.isColliding)
        //{
        //    Bridge b = raycastHits[0].collider.gameObject.GetComponent<Bridge>();
        //    if (collPacket_ground.collider.gameObject.layer == 12 && b != null && b.direction != 0.0f)
        //    {
        //        transform.Translate(new Vector2(b.openingSpeed * b.direction * Time.deltaTime, 0.0f));
        //    }
        //    else
        //    {
        //        transform.parent = null;
        //    }
        //}

        Animations();

        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();

        //flow testing mode
        if (Input.GetButton("Jump") && Input.GetButton("Flow") && Input.GetButton("SwapWeapon")) flowTestMode = !flowTestMode;
    }

    void SetPlayerState()
    {
        if (collPacket_ground.isColliding)
        {
            if (rb.velocity.sqrMagnitude > 0)
            {
                state = PlayerState.running;
            }
            else
            {
                state = PlayerState.idle;
            }
        }
        else
        {
            if(collPacket_frontLegs.isColliding && !collPacket_frontTorso.isColliding)
            {
                state = PlayerState.wallSticking;
            }
            else if (collPacket_backLegs.isColliding)
            {
                state = PlayerState.wallSticking;
            }
            else
            {
                state = PlayerState.jumping;
            }
        }
        
    }

    void HandleFlow()
    {
        //if flow button is pressed and player is not in flow and player has a full flow bar
        if (Input.GetButtonDown("Flow") && !inFlow && flowJuice == 1.0f)
        {
            FlowChange();
        }
        else if (Input.GetButtonDown("Flow") && inFlow)
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
        if (flowTestMode && !inFlow)
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

        while ((lerpUp && lerpVal < 1.0f) || (!lerpUp && lerpVal > 0.0f))
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
        float hInput = Input.GetAxis("LeftHorizontal");
        rb.AddForce(new Vector2(hInput * acceleration * movementLimiter, 0.0f));

        if (Input.GetAxis("LeftHorizontal") < 0.05f && Input.GetAxis("LeftHorizontal") > -0.05f)
        {
            if (state == PlayerState.running || inFlow)
            {
                //apply friction to the left
                if (acceleration != 0)
                {
                    Vector2 velocity = rb.velocity;
                    velocity.x *= 1 / acceleration;
                    rb.velocity = velocity;
                }

                //if (rb.velocity.x < 0)
                //{
                //    rb.velocity = new Vector2(0, rb.velocity.y);
                //}
            }
        }
        //else
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        if (rb.velocity.x > 0 && transform.localScale.x != 1)
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        else if (rb.velocity.x < 0&& transform.localScale.x != -1)
            gameObject.transform.localScale = new Vector3(-1, 1, 1);

    }

    IEnumerator DisableHorizontalMovement()
    {
        //limitHorizontalMovement = false;
        yield return new WaitForSeconds(0.75f);
        //limitHorizontalMovement = true;

    }

    void UpdatePlayerDirection()
    {
        float x;

        if (collPacket_frontLegs.isColliding && collPacket_frontTorso.isColliding)
        {
            direction.x *= -1;
        }
        ///remove this else if to return to ledge gripping
        else if(collPacket_frontLegs.isColliding || collPacket_frontTorso.isColliding)
        {
            direction.x *= -1;
        }
        else if (Mathf.Abs(Input.GetAxis("RightHorizontal")) > 0 && !collPacket_backLegs.isColliding)
        {
            x = Input.GetAxis("RightHorizontal");
            direction.x = x > 0 ? 1 : -1;
        }
        else if (Mathf.Abs(Input.GetAxis("LeftHorizontal")) > 0 && !collPacket_backLegs.isColliding)
        {
            x = Input.GetAxis("LeftHorizontal");
            direction.x = x > 0 ? 1 : -1;
        }

        if (collPacket_backLegs.isColliding && !collPacket_ground.isColliding)
        {
            isOnWall = true;
        }
        else if (isOnWall)
        {
            isOnWall = false;
        }

        gameObject.transform.localScale = direction;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (state == PlayerState.idle || state == PlayerState.running)
            {
                rb.AddForce(new Vector2(0.0f, jumpForce));
            }
            else if (state == PlayerState.wallSticking)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(horizontalForce * direction.x, jumpForce / wallJumpLimiter));
                CancelWallStick();
            }
        }
    }


    void Animations()
    {
        if (collPacket_ground.isColliding && !anim_legs.GetBool("onGround")) anim_legs.SetBool("onGround", true);
        else if (!collPacket_ground.isColliding && anim_legs.GetBool("onGround")) anim_legs.SetBool("onGround", false);
        if (collPacket_ground.isColliding && !anim_arms.GetBool("onGround")) anim_arms.SetBool("onGround", true);
        else if (!collPacket_ground.isColliding && anim_arms.GetBool("onGround")) anim_arms.SetBool("onGround", false);

        float speed = Mathf.Abs(rb.velocity.x);
        anim_legs.SetFloat("speed", speed);
        anim_arms.SetFloat("speed", speed);
        anim_spine.SetFloat("speed", speed);

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
        if (collPacket_backLegs.isColliding && !collPacket_ground.isColliding) isOnWall = true;
        if(state == PlayerState.ledgeGrabbing)
        {
            //rb.velocity = Vector2.zero;
            //rb.gravityScale = 0.0f;
        }
        if (isOnWall && !wallStickIsRunning)
        {
            StartCoroutine(WallStick());
        }
    }

    IEnumerator WallStick()
    {
        wallStickIsRunning = true;
        while (!collPacket_ground.isColliding && collPacket_backLegs.isColliding)
        {
            if (isFalling)
            {
                rb.gravityScale = 0.25f;
                acceleration = 0;
                movementLimiter = 0.0f;
                yield return new WaitForSeconds(stickTime);
                if (inFlow) acceleration = acceleration_fast;
                else acceleration = acceleration_slow;
                movementLimiter = 1.0f;
            }
            yield return null;
        }
        wallStickIsRunning = false;
        yield return null;
        //while(rb.gravityScale < 1.0f)
        //{
        //    rb.gravityScale += Time.deltaTime * stickiness;
        //    if (rb.gravityScale > 1.0f) rb.gravityScale = 1.0f;
        //    yield return null;
        //}
    }

    void CancelWallStick()
    {
        StopCoroutine(WallStick());
        rb.gravityScale = 1.0f;
        movementLimiter = 1.0f;
        wallStickIsRunning = false;
    }




    void GetCollisionReportFrontLegs(CollisionPacket packet)
    {
        collPacket_frontLegs = packet;
    }

    void GetCollisionReportBackLegs(CollisionPacket packet)
    {
        collPacket_backLegs = packet;
    }

    void GetCollisionReportFrontTorso(CollisionPacket packet)
    {
        collPacket_frontTorso = packet;
    }


    void GetCollisionReportGround(CollisionPacket packet)
    {
        collPacket_ground = packet;
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
            if (!collPacket_ground.isColliding)
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
