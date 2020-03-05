using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Flow Testing Mode")]
    [SerializeField] bool flowTestMode = false;

    [Header("Flow")]
    [SerializeField] float flowForgivenessTime = 1.0f; //how much time can the player stand still before they start losing flow
    [SerializeField] float flowDepreciationRate = 1.0f; //how fast the flow drains when player is moving too slow
    [SerializeField] float flowAppreciationRate = 1.0f; //how fast flow returns to the player when they are moving fast enough

    [Header("Movement - Flow")]
    [SerializeField] float acceleration_fast = 8;
    [SerializeField] float maxSpeed_fast = 10;
    [SerializeField] float jumpForce_fast = 250;

    [Header("Movement - No Flow")]
    [SerializeField] float acceleration_slow = 4;
    [SerializeField] float maxSpeed_slow = 5;
    [SerializeField] float jumpForce_slow = 200;

    [Header("Wall sticking")]
    [SerializeField] float stickTime = 1.0f; //how long the player's movement and acceleration are limited for after hitting a wall, used to give forgiveness to direction changes during a wall jump
    [SerializeField] float horizontalForce = 50; //how much force is applied horizontally after jumping off a wall
    [SerializeField] float wallJumpLimiter = 0.5f; //how much force is taken away from the verticality of a wall jump

    [Header("References :(")]
    [SerializeField] Slider flowSlider = null; //UI element for flow

    Rigidbody2D rb;
    public Vector2 GetVelocity() { return rb.velocity; }
    public Vector2 GetDirection() { return rb.velocity.normalized; }

    public Controls controls = null;

    float flowJuice = 1.0f; //how much spendable flow the player has
    public bool inFlow = false;
    float flowTimer = 0.0f; //keeps track of how long the player has been stalled for
    float flowThreshold = 0.0f; //how slow the player needs to be going to lose flow

    enum PlayerState
    {
        idle,
        running,
        jumping,
        wallSticking
    }
    PlayerState state;

    float acceleration; //player's current acceleration, swaps between acc_fast and acc_slow
    float maxSpeed; //players current maximum speed, swaps between maxSpeed_fast and maxSpeed_slow
    float jumpForce; //players current jump force, swaps between jump_fast and jump_slow

    public Vector2 direction = Vector2.zero; //the direction (usually left or right) that the player is facing

    //Collision packets recieving information on collisions with player's immediate surroundings
    CollisionPacket collPacket_ground; //reports on ground collisions
    CollisionPacket collPacket_frontLegs; //reports on collisions in front of player
    CollisionPacket collPacket_backLegs; //reports on collisions behind player

    float movementLimiter = 1.0f; //multiplier applied to player's horizontal input

    //Animators for each of the player's animatable child objects
    Animator animator;

    SoundManager sound;

    private void Awake()
    {
        controls = new Controls();
        controls.Player.Jump.performed += jump => Jump();
        controls.Player.Flow.performed += flow => TryFlow();
    }

    // Start is called before the first frame update
    void Start()
    {
        //get component references
        animator = GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        sound = FindObjectOfType<SoundManager>();

        //set flow variables
        inFlow = false;
        acceleration = acceleration_slow;
        maxSpeed = maxSpeed_slow;
        jumpForce = jumpForce_slow;
        flowThreshold = maxSpeed_slow * maxSpeed_slow * 0.5f;

        //set misc. player info
        state = PlayerState.idle;
        direction = new Vector2(1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        HandleFlow();
        JoystickMovement();
        SetPlayerState();
        UpdateAnimation();
    }

    public void EnableFire()
    {
        GetComponentInChildren<Bow>().EnableFire();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    /// <summary>
    /// Sets the PlayerState enum based on player's currect situation
    /// </summary>
    void SetPlayerState()
    {
        // if player is on ground
        if (collPacket_ground.isColliding)
        {
            //if player is moving, state = running
            if (rb.velocity.sqrMagnitude > 0)
            {
                state = PlayerState.running;
            }
            //if player is not running, state = idle
            else
            {
                state = PlayerState.idle;
            }
        }
        else
        {
            //if player has a wall immediately behind them, state = wall sticking
            if (collPacket_backLegs.isColliding)
            {
                state = PlayerState.wallSticking;
            }
            else
            {
                state = PlayerState.jumping;
            }
        }
    }

    /// <summary>
    /// Handles logic surrounding flow state, including switching states and managing states
    /// </summary>
    void HandleFlow()
    {
        //if player is in flow and it not moving fast enough, start countdown to losing flow
        if (rb.velocity.sqrMagnitude < flowThreshold)
        {
            StartCoroutine(FlowCountdown());
        }

        //if player is in flow and moving fast enough
        if (rb.velocity.sqrMagnitude >= flowThreshold && flowJuice < 1.0f)
        {
            //reset flow countdown
            StopCoroutine(FlowCountdown());
            flowTimer = flowForgivenessTime;
            flowJuice += Time.deltaTime * flowAppreciationRate;
            if (flowJuice > 1.0f) flowJuice = 1.0f;
        }
        //if flow forgiveness timer is up
        else if (flowTimer == 0 && flowJuice > 0.0f && inFlow)
        {
            //depreciate flow
            flowJuice -= Time.deltaTime * flowDepreciationRate;
            if (flowJuice < 0.0f) flowJuice = 0.0f;
        }


        //if player is in flow and is out of flow resource, end flow
        if (flowJuice == 0 && inFlow)
        {
            FlowChange();
        }

        flowSlider.value = flowJuice;


        //flow testing mode
        //if (Input.GetButton("Jump") && Input.GetButton("Flow") && Input.GetButton("SwapWeapon")) flowTestMode = !flowTestMode;
        //if (flowTestMode && !inFlow)
        //{
        //    FlowChange();
        //}
    }

    void TryFlow()
    {
        //if flow button is pressed and player is not in flow and player has a full flow bar
        if (!inFlow && flowJuice != 0.0f)
        {
            FlowChange();
        }
        //if flow button is pressed and player is in flow
        else if (inFlow)
        {
            FlowChange();
        }
    }

    /// <summary>
    /// Gives a delay of flowForgivenessTime seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator FlowCountdown()
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

        if (inFlow) StartCoroutine(FlerpToFlow());
        else StartCoroutine(FlerpToReality());
    }

    /// <summary>
    /// Creates a linear interpolation between player's reality and flow state
    /// </summary>
    /// <param name="lerpUp"></param>
    /// <returns></returns>
    IEnumerator FlerpToFlow()
    {
        StopCoroutine(FlerpToReality());

        float lerpVal = Mathf.InverseLerp(acceleration_slow, acceleration_fast, acceleration);

        //if lerping up, while value is below 1     if lerping down, while value is above 0
        while (lerpVal < 1.0f && inFlow)
        {
            lerpVal += Time.deltaTime;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            //lerp acceleration, maximum speed, and jump
            acceleration = Mathf.Lerp(acceleration_slow, acceleration_fast, lerpVal);
            maxSpeed = Mathf.Lerp(maxSpeed_slow, maxSpeed_fast, lerpVal);
            jumpForce = Mathf.Lerp(jumpForce_slow, jumpForce_fast, lerpVal);

            yield return null;
        }
    }

    IEnumerator FlerpToReality()
    {
        StopCoroutine(FlerpToFlow());
        float lerpVal = Mathf.InverseLerp(acceleration_slow, acceleration_fast, acceleration);

        while (lerpVal > 0.0f && !inFlow)
        {
            lerpVal -= Time.deltaTime;
            if (lerpVal < 0.0f) lerpVal = 0.0f;

            acceleration = Mathf.Lerp(acceleration_slow, acceleration_fast, lerpVal);
            maxSpeed = Mathf.Lerp(maxSpeed_slow, maxSpeed_fast, lerpVal);
            jumpForce = Mathf.Lerp(jumpForce_slow, jumpForce_fast, lerpVal);

            yield return null;
        }
    }

    /// <summary>
    /// Translate's player's joystick input to player character's movement
    /// </summary>
    void JoystickMovement()
    {
        //add a force to the player based on horizontal movement of the left joystick, the player's currect acceleration, and the movement limiter factor
        float inputLeft = controls.Player.Move.ReadValue<float>();
        float inputRight = controls.Player.Aim.ReadValue<Vector2>().x;
        rb.AddForce(new Vector2(inputLeft * acceleration * movementLimiter, 0.0f));
        if (!collPacket_backLegs.isColliding && ((direction.x > 0 && inputLeft < 0) || (direction.x < 0 && inputLeft > 0)))
        {
            direction.x *= -1;
        }

        //if player is giving little to no input to the horizontal component of the left joystick
        if (Mathf.Abs(inputLeft) < 0.05f && (collPacket_ground.isColliding || inFlow))
        {
            //apply friction
            if (acceleration != 0)
            {
                Vector2 velocity = rb.velocity;
                velocity.x *= 1 / acceleration;
                rb.velocity = velocity;
            }
        }

        //cap velocity between max speed and negative max speed
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);


        //if the player just ran into a wall, turn around
        if (collPacket_frontLegs.isColliding)
        {
            direction.x *= -1;
        }
        //if player is aim left or right and is not on a wall, set direction.x to x component of player's aim
        else if (Mathf.Abs(inputRight) > 0 && !collPacket_backLegs.isColliding)
        {
            direction.x = inputRight > 0 ? 1 : -1;
        }
        //update player's scale to reflect their direction
        if((Vector2)transform.localScale != direction) transform.localScale = direction;
        //float bowX = FindObjectOfType<Bow>().direction.x;
        //if ((bowX < 0 && direction.x > 0) || (bowX > 0 && direction.x < 0)) FindObjectOfType<Bow>().FlipDirection();
        
    }

    void Movement(float direction)
    {
        rb.AddForce(new Vector2(direction * acceleration * movementLimiter, 0.0f));
    }


    /// <summary>
    /// Translates player's jump input to player character jumping
    /// </summary>
    void Jump()
    {
        if (state == PlayerState.idle || state == PlayerState.running)
        {
            //apply upward force to player
            rb.AddForce(new Vector2(0.0f, jumpForce));
        }
        else if (state == PlayerState.wallSticking)
        {
            //negate player's current momentum, apply force upwards and away from the wall
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(horizontalForce * direction.x, jumpForce / wallJumpLimiter));
            CancelWallStick();
        }
    }


    /// <summary>
    /// Reports player information to animator state machine
    /// </summary>
    void UpdateAnimation()
    {
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));
    }


    /// <summary>
    /// Applies wall stick logic to player
    /// </summary>
    /// <returns></returns>
    IEnumerator WallStick()
    {
        StartCoroutine(Rumble.BurstRumbleContinuous(0.5f, 0.1f, 0.05f));
        //while player is off ground and on wall
        while (!collPacket_ground.isColliding && collPacket_backLegs.isColliding)
        {
            //if player is falling
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = 0.25f;

                //disable horizontal movement for stickTime seconds (gives player a chance to turn around without leaving the wall
                acceleration = 0;
                movementLimiter = 0.0f;
                yield return new WaitForSeconds(stickTime);
                acceleration = inFlow ? acceleration_fast : acceleration_slow;
                movementLimiter = 1.0f;
            }
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// Disables all logic dealing with wall stick
    /// </summary>
    void CancelWallStick()
    {
        StopCoroutine(WallStick());
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
        rb.gravityScale = 1.0f;
        movementLimiter = 1.0f;
    }



    /// <summary>
    /// recieves collision packet from front of player
    /// </summary>
    /// <param name="packet"></param>
    void GetCollisionReportFrontLegs(CollisionPacket packet)
    {
        if (!packet.collider.isTrigger)
            collPacket_frontLegs = packet;
    }

    /// <summary>
    /// recieves a collision packet from behind player
    /// </summary>
    /// <param name="packet"></param>
    void GetCollisionReportBackLegs(CollisionPacket packet)
    {
        collPacket_backLegs = packet;

        if (!packet.isColliding)
        {
            rb.gravityScale = 1.0f;
            movementLimiter = 1.0f;
            CancelWallStick();
        }
        else if (!collPacket_ground.isColliding)
        {
            StartCoroutine(WallStick());
        }

        animator.SetBool("onWall", packet.isColliding);
    }

    /// <summary>
    /// recieves a collision packet from below player
    /// </summary>
    /// <param name="packet"></param>
    void GetCollisionReportGround(CollisionPacket packet)
    {
        collPacket_ground = packet;
        if (packet.isColliding)
        {
            CancelWallStick();
            StartCoroutine(Rumble.BurstRumble(0.5f, 0.1f));
        }

        animator.SetBool("onGround", packet.isColliding);
    }

    /// <summary>
    /// slows the player down to set speed and jump for specified amount of time
    /// </summary>
    /// <param name="newMaxSpeed">the player's slowed speed</param>
    /// <param name="newJumpForce">the player's slowed jump force</param>
    /// <param name="time">length of slowness in seconds</param>
    /// <returns></returns>
    public void ReduceMovement(float p_maxSpeed, float p_jumpForce, float time)
    {
        StartCoroutine(C_ReduceMovement(p_maxSpeed, p_jumpForce, time));
    }

    IEnumerator C_ReduceMovement(float p_maxSpeed, float p_jumpForce, float time)
    {
        maxSpeed = p_maxSpeed;
        jumpForce = p_jumpForce;
        yield return new WaitForSecondsRealtime(time);
        maxSpeed = inFlow ? maxSpeed_fast : maxSpeed_slow;
        jumpForce = inFlow ? jumpForce_fast : jumpForce_slow;
    }

    /// <summary>
    /// applies the given force to the player
    /// </summary>
    /// <param name="knockback">the force to be applied to the player</param>
    /// <param name="groundRequired">does the player need to be on the ground?</param>
    public void AddKnockback(Vector2 knockback, bool airOnly)
    {
        rb.velocity = Vector2.zero;
        //if knockback is marked as disabled on ground, only apply force if player is in the air
        if (airOnly)
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

    public void PlayWalkSound()
    {
        string[] walkSounds = new string[3] { "Walk1", "Walk2", "Walk3" };
        sound.PlayRandom(walkSounds, 0.9f, 1.1f);
    }

    /// <summary>
    /// helper function to RigidBody.AddForce to allow seperated components without having to manually make a new vector
    /// </summary>
    /// <param name="x">force in the x direction</param>
    /// <param name="y">force in the y direction</param>
    private void AddForce(float x, float y) { rb.AddForce(new Vector2(x, y)); }
}