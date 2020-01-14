using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float gravity = 0.01f;

    protected Vector2 position;
    protected Vector2 velocity;
    protected Vector2 acceleration;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
        if (DetectGroudHit())
        {
            velocity.y = 0;
        }
        velocity += acceleration;
        position += velocity;
        transform.position = position;
    }

    void ApplyGravity()
    { 
        acceleration.y += gravity;
    }

    bool DetectGroudHit()
    {
        Debug.DrawLine(new Vector2(position.x, position.y - gameObject.GetComponent<CapsuleCollider2D>().bounds.size.y / 2), Vector3.down * velocity.magnitude);
        if (Physics2D.Raycast(new Vector2(position.x, position.y - gameObject.GetComponent<CapsuleCollider2D>().bounds.size.y / 2), Vector2.down, velocity.magnitude, 1 << 9 | 1 << 11).collider != null)
        {
            return true;
        }
        
        return false;
    }
}
