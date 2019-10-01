using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float GRAVITY = 1f;

    Vector2 position;
    Vector2 velocity;
    Vector2 acceleration;

    RaycastHit2D[] raycastHits;

    [SerializeField] float mass;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float stickiness; // smaller = more sticky
    [SerializeField] float coefFriction;

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

    void HeadHit()
    {
        if(raycastHits[3].collider != null && !isFalling)
        {
            acceleration.y = 0;
            isFalling = true;
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

        if(raycastHits[3].collider != null)
        {
            position.y = raycastHits[3].collider.transform.position.y -
                raycastHits[3].collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 -
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }
    }

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
