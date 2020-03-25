using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    [SerializeField] protected float mass;
    [SerializeField] protected float maxSpeed;
    protected Rigidbody2D rb;
    protected Vector3 velocity;
    protected Vector3 acceleration;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //rb.MovePosition(transform.position + velocity);
        velocity += acceleration;
        Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        Mathf.Clamp(velocity.y, -maxSpeed, maxSpeed);
        //transform.Translate(velocity);
        rb.MovePosition(rb.position + (Vector2)velocity);
        acceleration = Vector3.zero;
    }

    protected Vector3 GetSeekForce(Vector3 target)
    {
        return ((target - transform.position).normalized * maxSpeed) - velocity;
    }

    protected Vector3 GetFleeForce(Vector3 target)
    {
        return ((transform.position - target).normalized * maxSpeed) - velocity;
    }

    protected void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }
}
