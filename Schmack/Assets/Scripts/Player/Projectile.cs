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
        Debug.Log("arrow start");
    }

    // Update is called once per frame
    protected void Update()
    {
        if (rb)
        {
            rotation.x = rb.velocity.normalized.x;
            rotation.y = rb.velocity.normalized.y;
        }
        transform.rotation = rotation;
    }
}
