using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    [SerializeField] protected float mass;
    [SerializeField] protected float maxSpeed;
    protected Rigidbody2D rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected Vector3 Seek(Vector3 target)
    {
        return ((target - transform.position).normalized * maxSpeed * Time.deltaTime) / mass;
    }
}
