using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Vector2 direction;
    protected Rigidbody2D rb;
    protected Quaternion rotation;

    protected void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();

    }

    // Start is called before the first frame update
    protected void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        if (rb)
        {
            transform.LookAt((Vector2)transform.position + rb.velocity);
            //transform.right = rb.velocity;
        }
    }
}
