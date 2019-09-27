using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float GRAVITY = 1.0f;

    //physics vectors
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 acceleration;

    [SerializeField] float mass;
    [SerializeField] float speed;   //determines how much standard joystick controls move the player (horizontal)
    [SerializeField] float jumpForce;   //determines how much power is behind the player's jump
    [SerializeField] float stickiness; //between 0 and 1, lower = stickier, determines how much wall sticking slows the player's descent
    [SerializeField] float coefFriction;    //determines how much friction effects the player

    //flags
    bool isJumping = false;
    bool isSticking = false;
    bool isInAir = false;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //reset velocity
        velocity = Vector2.zero;

        //environmental forces
        Gravity();
        AddFriction();
        WallStick();

        //player controls
        float h = Input.GetAxis("LeftHorizontal");
        velocity.x = h * speed;

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            isJumping = true;
            acceleration.y = 0; //cancel out any previous forces
            AddForce(0, jumpForce);
        }

        //application of forces
        velocity += acceleration * Time.deltaTime;
        position += velocity;
        transform.position = position;
    }

    /// <summary>
    /// Applies the force of gravity whenever necessary
    /// </summary>
    void Gravity()
    {
        //cast a ray downward from the player
        LayerMask layerMask = LayerMask.GetMask("environment"); //specifies that only objects in the environment layer can be landed on
        RaycastHit2D hit = CreateDynamicRaycast(Vector2.down, layerMask);

        //if the ray hits a platform
        if (hit.collider != null)
        {
            isJumping = false;
            isSticking = false;
            isInAir = false;

            acceleration.y = 0;
            //snap player to the top of the platform
            position.y = hit.collider.transform.position.y +
                hit.collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 +
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }
        else if(!isSticking)
        {
            isInAir = true;
            acceleration.y -= GRAVITY;
        }
        else
        {
            isInAir = true;
        }
    }

    /// <summary>
    /// Slows the player's downward velocity if they are next to a wall
    /// </summary>
    void WallStick()
    {
        //cast a ray to the left and right of the player
        LayerMask layerMask = LayerMask.GetMask("environment");
        RaycastHit2D leftHit = CreateDynamicRaycast(Vector2.left, layerMask); //left
        RaycastHit2D rightHit = CreateDynamicRaycast(Vector2.right, layerMask); //right

        //if the ray hits a platform to the left
        if(leftHit.collider != null)
        {
            isJumping = false;
            isSticking = true;

            //slow down any leftover upward acceleration
            if (acceleration.y > 0)
            {
                acceleration.y /= 1.3f;
            }

            //apply a force downward
            float downwardForce = -GRAVITY * stickiness;
            AddForce(0, downwardForce);

            //snap player to the right of the platform
            position.x = leftHit.collider.transform.position.x + 
                leftHit.collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 + 
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }
        //the the ray hits a platform to the right
        else if(rightHit.collider != null){
            isJumping = false;
            isSticking = true;

            //slow down any leftover upward acceleration
            if (acceleration.y > 0)
            {
                acceleration.y /= 1.3f;
            }

            //apply a force downward
            float downwardForce = -GRAVITY + stickiness;
            AddForce(0, downwardForce);

            //snap the player to the left of the platform
            position.x = rightHit.collider.transform.position.x -
                rightHit.collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 -
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }
        else
        {
            isSticking = false;
        }
    }

    /// <summary>
    /// Applies friction is thep layer is standing on a platform
    /// </summary>
    void AddFriction()
    {
        if (!isInAir && Mathf.Abs(acceleration.x) > 0)
        {
            AddForce(-acceleration.x * coefFriction, 0);
        }
    }

    /// <summary>
    /// applies force in opposite direction of bow fire
    /// </summary>
    /// <param name="direction">unit vector, the direction the bow is pointed in</param>
    /// <param name="force">the amount of force applied by the knockback</param>
    public void AddBowKnockback(Vector2 direction, float force)
    {
        AddForce(-direction * force);
    }

    /// <summary>
    /// applies a force to acceleration
    /// </summary>
    /// <param name="force">the amount force applied to the player</param>
    void AddForce(Vector2 force)
    {
        acceleration += force / mass;
    }

    /// <summary>
    /// applies a force to acceleration
    /// </summary>
    /// <param name="x">amount of force applied in the x direction</param>
    /// <param name="y">amount of force applied in the y direction</param>
    void AddForce(float x, float y)
    {
        acceleration.x += x / mass;
        acceleration.y += y / mass;
    }

    /// <summary>
    /// casts a ray with a size depenant on the player's current velocity
    /// </summary>
    /// <param name="direction">unit vector, the direction the ray will be casted in</param>
    /// <param name="layerMask">LayerMask including all layers that should be checked by the Raycast</param>
    /// <returns></returns>
    RaycastHit2D CreateDynamicRaycast(Vector2 direction, LayerMask layerMask)
    {
        float distance = Mathf.Clamp(velocity.sqrMagnitude * 2.5f, 0.5f, float.MaxValue);
        return Physics2D.Raycast(position, direction, distance, layerMask);
    }
}
