using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const float GRAVITY = 1.0f;

    private Vector2 position;
    private Vector2 velocity;
    private Vector2 acceleration;
    [SerializeField] float mass;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float stickiness; //between 0 and 1
    [SerializeField] float coefFriction;
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
        velocity = Vector2.zero;
        Gravity();
        WallStick();
        AddFriction();

        float h = Input.GetAxis("LeftHorizontal");
        velocity.x = h * speed;

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            acceleration.y = 0; //cancel out any previous forces
            AddForce(0, jumpForce);
            isJumping = true;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity;
        transform.position = position;
    }

    /// <summary>
    /// Applies the force of gravity whenever necessary
    /// </summary>
    void Gravity()
    {
        int layerMask = ~(1 << 8); //excludes the player layer from raycasts
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 0.5f, layerMask);
        if (hit.collider != null)
        {
            acceleration.y = 0;
            isJumping = false;
            isSticking = false;
            isInAir = false;
            position.y = hit.collider.transform.position.y +
                hit.collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 +
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }
        else if(!isSticking)
        {
            acceleration.y -= GRAVITY;
            isInAir = true;
        }
        else
        {
            isInAir = true;
        }
    }

    void WallStick()
    {
        int layerMask = ~(1 << 8); //excludes the player from raycasts
        RaycastHit2D leftHit = Physics2D.Raycast(position, Vector2.left, 0.5f, layerMask);
        RaycastHit2D rightHit = Physics2D.Raycast(position, Vector2.right, 0.5f, layerMask);
        if(leftHit.collider != null)
        {
            if(acceleration.y > 0)
            {
                acceleration.y /= 1.3f;
            }
            isJumping = false;
            isSticking = true;
            float downwardForce = -GRAVITY * stickiness;
            AddForce(0, downwardForce);
            position.x = leftHit.collider.transform.position.x + 
                leftHit.collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 + 
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }
        else if(rightHit.collider != null){
            if (acceleration.y > 0)
            {
                acceleration.y /= 1.3f;
            }
            isJumping = false;
            isSticking = true;
            float downwardForce = -GRAVITY + stickiness;
            AddForce(0, downwardForce);
            position.x = rightHit.collider.transform.position.x -
                rightHit.collider.GetComponent<BoxCollider2D>().bounds.size.x / 2 -
                gameObject.GetComponent<BoxCollider2D>().bounds.size.x / 2;
        }
        else
        {
            isSticking = false;
        }
    }

    void AddFriction()
    {
        Debug.Log(isInAir);
        Debug.Log(Mathf.Abs(velocity.x));
        if (!isInAir && Mathf.Abs(acceleration.x) > 0)
        {
            AddForce(-acceleration.x * coefFriction, 0);
            Debug.Log(velocity);
        }
    }

    public void AddBowKnockback(Vector2 direction, float force)
    {
        AddForce(direction * force);
    }

    void AddForce(Vector2 force)
    {
        acceleration += force / mass;
    }
    void AddForce(float x, float y)
    {
        acceleration.x += x / mass;
        acceleration.y += y / mass;
    }
}
