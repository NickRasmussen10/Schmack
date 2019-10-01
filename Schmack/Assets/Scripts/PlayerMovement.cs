using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float GRAVITY = 1f;

    //physics vectors
    Vector2 position;
    Vector2 velocity;
    Vector2 acceleration;

    //array to keep track of collisions in all 4 cardinal directions
    RaycastHit2D[] raycastHits;

    //physics/platforming variables
    [SerializeField] float mass;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float stickiness; // smaller = more sticky
    [SerializeField] float coefFriction;

    //bool flags to know what the player's current state is
    bool isSticking = false;
    bool isJumping = false;
    bool isFalling = true;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        acceleration = Vector2.zero;
        raycastHits = new RaycastHit2D[4];
        SetRaycasts(LayerMask.GetMask("environment"));

    }

    // Update is called once per frame
    void Update()
    {
        velocity += acceleration * Time.deltaTime;
        position += velocity;
        SetRaycasts(LayerMask.GetMask("environment"));
        WallSticking();
        Gravity();
        HeadHit();
        BasicControls();
        AddFriction();

        velocity = Vector2.zero;

        SnapPositions();
        transform.position = position;
    }

    /// <summary>
    /// handles the basic controller input for the player (horizontal movemnet, jumping)
    /// </summary>
    void BasicControls()
    {
        float horizontalInput = Input.GetAxis("LeftHorizontal");
        acceleration.x = horizontalInput * speed;
        if(raycastHits[1].collider != null && acceleration.x < 0)
        {
            acceleration.x = 0;
        }
        if(raycastHits[2].collider != null && acceleration.x > 0)
        {
            acceleration.x = 0;
        }

        if(Input.GetButtonDown("Jump") && (!isJumping || isSticking))
        {
            AddForce(0, jumpForce);
            isJumping = true;
        }
    }

    /// <summary>
    /// raises isSticking flag when player is next to a wall
    /// </summary>
    void WallSticking()
    {
        if(raycastHits[1].collider != null || raycastHits[2].collider != null)
        {
            isSticking = true;
        }
        else
        {
            isSticking = false;
        }
    }


    /// <summary>
    /// handles player falling to the ground, including slower fall for wall sticking
    /// </summary>
    void Gravity()
    {
        if(raycastHits[0].collider != null)
        {
            isFalling = false;
            isJumping = false;
            acceleration.y = 0;
        }
        else if(raycastHits[0].collider == null && velocity.y == 0)
        {
            isFalling = true;
        }


        if (isFalling && isSticking)
        {
            //slower descent
            acceleration.y -= GRAVITY * stickiness;
        }
        else if(raycastHits[0].collider == null)
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
    /// applies a force to slow horizontal motion if player is on the ground
    /// </summary>
    void AddFriction()
    {
        if(raycastHits[0].collider != null && Mathf.Abs(acceleration.x) > 0)
        {
            AddForce(-velocity.x * coefFriction, 0);
        }
    }

    /// <summary>
    /// snaps the player's position to perfectly adjacent to any platform they are colliding with (avoids overlapping)
    /// </summary>
    void SnapPositions()
    {
        if(raycastHits[0].collider != null)
        {
            position.y = raycastHits[0].collider.transform.position.y +
                raycastHits[0].collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 +
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }

        if(raycastHits[1].collider != null)
        {
            position.x = raycastHits[1].collider.transform.position.x +
                raycastHits[1].collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 +
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }

        if(raycastHits[2].collider != null)
        {
            position.x = raycastHits[2].collider.transform.position.x -
                raycastHits[2].collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 -
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }

        if(raycastHits[3].collider != null)
        {
            position.y = raycastHits[3].collider.transform.position.y -
                raycastHits[3].collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 -
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }
    }


    /// <summary>
    /// casts rays in all 4 cardinal directions from player for collision detection
    /// </summary>
    /// <param name="layerMask"></param>
    void SetRaycasts(LayerMask layerMask)
    {
        if (isFalling)
        {
            Vector3 dest = new Vector3();
            dest = transform.position + (Vector3.down * (0.5f - velocity.y));
            Debug.DrawLine(position, dest, Color.magenta);
            raycastHits[0] = Physics2D.Raycast(position, Vector2.down, 0.5f - velocity.y, layerMask);
        }
        else
        {
            Vector3 dest = new Vector3();
            dest = transform.position + (Vector3.down * 0.5f);
            Debug.DrawLine(position, dest, Color.magenta);
            raycastHits[0] = Physics2D.Raycast(position, Vector2.down, 0.5f, layerMask);
        }

        Debug.DrawLine(position, position + Vector2.up * (0.5f + velocity.y), Color.magenta);
        raycastHits[3] = Physics2D.Raycast(position, Vector2.up, 0.5f + velocity.y, layerMask);

        
        

        if (raycastHits[1].collider != null)
        {
            Vector3 dest1 = new Vector3();
            dest1 = transform.position + (Vector3.left * 0.5f);
            Debug.DrawLine(position, dest1, Color.magenta);
            raycastHits[1] = Physics2D.Raycast(position, Vector2.left, 0.5f, layerMask);
        }
        else
        {
            Vector3 dest1 = new Vector3();
            dest1 = transform.position + (Vector3.left * (0.5f + velocity.x));
            Debug.DrawLine(position, dest1, Color.magenta);
            raycastHits[1] = Physics2D.Raycast(position, Vector2.left, 0.5f + velocity.x, layerMask);
        }

        if (raycastHits[2].collider != null)
        {
            Vector3 dest2 = new Vector3();
            dest2 = transform.position + (Vector3.right * 0.5f);
            Debug.DrawLine(position, dest2, Color.magenta);
            raycastHits[2] = Physics2D.Raycast(position, Vector2.right, 0.5f, layerMask);
        }
        else
        {
            Vector3 dest2 = new Vector3();
            dest2 = transform.position + (Vector3.right * (0.5f + velocity.x));
            Debug.DrawLine(position, dest2, Color.magenta);
            raycastHits[2] = Physics2D.Raycast(position, Vector2.right, 0.5f + velocity.x, layerMask);
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

    void AddForce(Vector2 force)
    {
        acceleration += force / mass;
    }

    void AddForce(float x, float y)
    {
        AddForce(new Vector2(x, y));
    }
}
