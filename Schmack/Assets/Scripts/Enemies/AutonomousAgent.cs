using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float mass;
    [SerializeField] float maxSpeed;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Seek(target.position));
    }

    Vector3 Seek(Vector3 target)
    {
        return ((target - transform.position) * maxSpeed * Time.deltaTime) / mass;
    }
}
