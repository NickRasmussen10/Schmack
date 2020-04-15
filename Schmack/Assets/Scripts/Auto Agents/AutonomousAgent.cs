﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    [SerializeField] protected float mass;
    [SerializeField] protected float maxSpeed;
    protected Rigidbody2D rb;
    public Vector3 velocity;
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
        velocity += acceleration;
        velocity *= Time.deltaTime;
        Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        Mathf.Clamp(velocity.y, -maxSpeed, maxSpeed);
        rb.MovePosition(rb.position + (Vector2)velocity);
        //if(rb.velocity.sqrMagnitude < maxSpeed * maxSpeed)
        //    rb.AddForce(velocity * 10);
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
