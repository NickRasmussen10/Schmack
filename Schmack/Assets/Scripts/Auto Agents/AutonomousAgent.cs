using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    [SerializeField] protected float mass;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float friction = 0.1f; // > 1 would make friction speed it up
    protected Rigidbody2D rb;
    public Vector3 innerVelocity;
    public Vector3 outerVelocity = Vector2.zero;
    public Vector3 acceleration;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        innerVelocity += acceleration;
        innerVelocity *= Time.deltaTime;
        Mathf.Clamp(innerVelocity.x, -maxSpeed, maxSpeed);
        Mathf.Clamp(innerVelocity.y, -maxSpeed, maxSpeed);
        rb.MovePosition(rb.position + (Vector2)innerVelocity + (Vector2)outerVelocity);

        if (outerVelocity.sqrMagnitude > 0) outerVelocity *= friction;

        //if(rb.velocity.sqrMagnitude < maxSpeed * maxSpeed)
        //    rb.AddForce(velocity * 10);
        acceleration = Vector3.zero;
    }

    protected Vector3 GetSeekForce(Vector3 target)
    {
        return ((target - transform.position).normalized * maxSpeed) - innerVelocity;
    }

    protected Vector3 GetFleeForce(Vector3 target)
    {
        return ((transform.position - target).normalized * maxSpeed) - innerVelocity;
    }

    protected void ApplyInnerForce(Vector3 force)
    {
        acceleration += force / mass;
    }
    
    public void ApplyForce(Vector3 force)
    {
        Debug.Log(force);
        outerVelocity += force;
    }
}
