using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float GRAVITY = 0.1f;

    Vector2 position;
    Vector2 velocity;
    Vector2 acceleration;

    Vector2 prev_velocity;

    RaycastHit2D[] raycastHits;

    [SerializeField] float mass;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float stickiness;
    [SerializeField] float coefFriction;

    bool isSticking = false;
    bool isJumping = false;
    bool isFalling = true;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        acceleration = Vector2.zero;
        raycastHits = new RaycastHit2D[3];
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector2.zero;

        SetRaycasts(LayerMask.GetMask("environment"));
        Gravity();
        BasicControls();
        WallSticking();
        AddFriction();
        SnapPositions();


        Debug.Log("isFalling: " + isFalling);

        velocity += acceleration;
        position += velocity;
        transform.position = position;
        prev_velocity = velocity;
    }

    void BasicControls()
    {
        float horizontalInput = Input.GetAxis("LeftHorizontal");
        velocity.x = horizontalInput * speed;

        if(Input.GetButtonDown("Jump") && (!isJumping || isSticking))
        {
            AddForce(0, jumpForce);
            isJumping = true;
        }
    }

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
            acceleration.y -= GRAVITY * stickiness;
        }
        else if(raycastHits[0].collider == null)
        {
            acceleration.y -= GRAVITY;
        }
    }

    void AddFriction()
    {
        if(raycastHits[0].collider != null && Mathf.Abs(acceleration.x) > 0)
        {
            AddForce(-velocity.x * coefFriction, 0);
        }
    }

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
    }

    void SetRaycasts(LayerMask layerMask)
    {
        if (isFalling)
        {
            raycastHits[0] = Physics2D.Raycast(position, Vector2.down, 0.5f + prev_velocity.y, layerMask);
        }
        else
        {
            raycastHits[0] = Physics2D.Raycast(position, Vector2.down, 0.5f, layerMask);
        }
        raycastHits[1] = Physics2D.Raycast(position, Vector2.left, 0.5f + prev_velocity.x, layerMask);
        raycastHits[2] = Physics2D.Raycast(position, Vector2.right, 0.5f + prev_velocity.x, layerMask);
    }

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
