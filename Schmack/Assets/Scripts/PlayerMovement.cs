using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Projectile
{
    //physics/platforming variables
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float stickiness; // smaller = more sticky

    //bool flags to know what the player's current state is
    bool isSticking = false;
    bool isJumping = false;
    bool isFalling = true;
    public bool GetIsOnGround() { if (raycastHits[0].collider != null) return true; return false; }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if(raycastHits[0].collider != null)
        {
            isJumping = false;
        }

        WallSticking();
        if (isSticking)
        {
            ResistGravity();
        }
        HeadHit();
        BasicControls();

        
    }

    /// <summary>
    /// handles the basic controller input for the player (horizontal movemnet, jumping)
    /// </summary>
    void BasicControls()
    {
        float horizontalInput = Input.GetAxis("LeftHorizontal");
        velocity.x = horizontalInput * speed;

        if (Input.GetButtonDown("Jump") && (!isJumping || isSticking))
        {
            Debug.Log("jump");
            AddForce(0, jumpForce);
            isJumping = true;
        }
    }

    /// <summary>
    /// raises isSticking flag when player is next to a wall
    /// </summary>
    void WallSticking()
    {
        if((raycastHits[1].collider != null || raycastHits[2].collider != null) && acceleration.y <= 0)
        {
            isSticking = true;
        }
        else
        {
            isSticking = false;
        }
    }

    void ResistGravity()
    {
        AddForce(0, GRAVITY * stickiness);
    }

    /// <summary>
    /// handles player falling to the ground, including slower fall for wall sticking
    /// </summary>
    void Gravity()
    {
        if (raycastHits[0].collider != null)
        {
            isFalling = false;
            isJumping = false;
            acceleration.y = 0;
        }
        else if (raycastHits[0].collider == null && velocity.y == 0)
        {
            isFalling = true;
        }


        if (isFalling && isSticking)
        {
            //slower descent
            acceleration.y -= GRAVITY * stickiness;
        }
        else if (raycastHits[0].collider == null)
        {
            acceleration.y -= GRAVITY;
        }
    }

    /// <summary>
    /// stops the player's ascent if they jump into the underside of a platform
    /// </summary>
    void HeadHit()
    {
        if(raycastHits[3].collider != null && !isFalling)
        {
            acceleration.y = 0;
            isFalling = true;
        }
    }

    /// <summary>
    /// applies knockback force to player
    /// </summary>
    /// <param name="direction">direction the bow is facing</param>
    /// <param name="force">how much knockback to apply</param>
    public void AddBowKnockback(Vector2 direction, float force)
    {
        AddForce(-direction * force);
    }
}
