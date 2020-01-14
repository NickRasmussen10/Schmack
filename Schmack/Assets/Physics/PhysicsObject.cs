using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    protected Vector3 position;
    protected Vector3 velocity;
    protected Vector3 acceleration;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity;
        velocity += acceleration;
        position += velocity;
        transform.position = position;
    }

    void ApplyGravity()
    {
        acceleration.y -= Physics2D.gravity.y;
    }
}
