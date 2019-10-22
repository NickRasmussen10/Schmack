using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Movement Variables")]
    [SerializeField] float acceleration = 8;
    [SerializeField] float maxSpeed = 10;
    [SerializeField] float jumpForce = 250;

    [Header("Wall sticking")]
    [SerializeField] float stickiness = 1.0f;
    [SerializeField] float horizontalForce = 50;
    [SerializeField] float wallJumpLimiter = 1.5f;

    RaycastHit2D[] raycastHits = new RaycastHit2D[3];

    bool isFalling = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(new Vector2(Input.GetAxis("LeftHorizontal") * acceleration, 0.0f));
        if (Input.GetAxis("LeftHorizontal") < 0.05f && Input.GetAxis("LeftHorizontal") > -0.05f)
        {
            if(rb.velocity.x > 0 && CheckRayCollision(0))
            {
                //apply friction to the left

                rb.AddForce(new Vector2(-acceleration, 0.0f));
                if(rb.velocity.x < 0)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            else if(rb.velocity.x < 0 && CheckRayCollision(0))
            {
                //apply friciton to the right

                rb.AddForce(new Vector2(acceleration, 0.0f));
                if (rb.velocity.x > 0)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
        }

        CastRays();
        if(!CheckRayCollision(0) && rb.velocity.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

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

        if ((CheckRayCollision(1) || CheckRayCollision(2)) && isFalling)
        {
            rb.AddForce(-Physics2D.gravity / stickiness);
            

        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
    }

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
    /// <param name="index"></param>
    /// <returns></returns>
    bool CheckRayCollision(int index)
    {
        //oof ouch owie my code
        return raycastHits[index].collider != null;
    }

    bool CheckWallStick()
    {
        return CheckRayCollision(1) || CheckRayCollision(2);
    }

    public void AddKnockback(Vector2 knockback, bool groundRequired)
    {
        if (groundRequired)
        {
            if (!CheckRayCollision(0))
            {
                AddForce(knockback);
            }
        }
        else
        {
            AddForce(knockback);
        }
        
    }

    private void AddForce(Vector2 force) { rb.AddForce(force); }
    private void AddForce(float x, float y) { rb.AddForce(new Vector2(x, y)); }
}
