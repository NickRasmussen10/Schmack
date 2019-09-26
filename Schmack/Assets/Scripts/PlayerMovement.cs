using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 acceleration;
    [SerializeField] float mass;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    bool isJumping = false;

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

        float h = Input.GetAxis("Horizontal");
        velocity.x = h * speed;
        Debug.Log(velocity);

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
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
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 0.6f, layerMask);
        if (hit.collider != null)
        {
            acceleration.y = 0;
            isJumping = false;
            position.y = hit.collider.transform.position.y +
                hit.collider.GetComponent<BoxCollider2D>().bounds.size.y / 2 +
                gameObject.GetComponent<BoxCollider2D>().bounds.size.y / 2;
        }
        else
        {
            acceleration.y--;
        }
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
