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
        
    }
}
