using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody2D rb;

    protected bool hasHit = false; //has this project hit a valid collider

    protected RaycastHit2D raycastHit; //ray that gets cast in front of projectile to detect oncoming collision

    protected int layermask = 1 << 9 | 1 << 11 | 1 << 12; //masks all layers except environment, enemies, and interactibles
    //NOTE: this specific layermask is for arrows and may be overwritten in child classes

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
        //if rigid body it not null
        if (rb)
            //raycast in direction of velocity
            raycastHit = Physics2D.Raycast(transform.position, rb.velocity.normalized, rb.velocity.sqrMagnitude * 0.001f, layermask);

        //if raycast detected hit and projectile has not yet hit something
        if(raycastHit.collider != null && !hasHit)
        {
            if (!raycastHit.collider.gameObject.GetComponent<CollisionReporter>())
            {
                //mark projectile as hit
                hasHit = true;

                TriggerHit(raycastHit.collider, raycastHit.point);
            }
        }
    }


    /// <summary>
    /// Handles logic for when the projectile detects collision with a valid object
    /// </summary>
    /// <param name="collision">the collider of the object that collision has been detected with</param>
    /// <param name="collisionPoint">the exact point at which the projectile hit the collider</param>
    protected abstract void TriggerHit(Collider2D collision, Vector3 collisionPoint);

    /// <summary>
    /// applies the given force to this projectile
    /// </summary>
    /// <param name="force"></param>
    public void AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }

    /// <summary>
    /// applies a force based on the given components to this projectile
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void AddForce(float x, float y)
    {
        AddForce(new Vector2(x, y));
    }
}
