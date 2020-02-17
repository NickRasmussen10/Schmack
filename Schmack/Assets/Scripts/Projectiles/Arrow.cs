﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    [SerializeField] TrailRenderer trail = null;

    // Start is called before the first frame update
    protected new virtual void Start()
    {
        base.Start();

        //start facing in direction of velocity
        transform.right = -rb.velocity;
        Instantiate(trail, transform);
    }

    // Update is called once per frame
    protected new virtual void Update()
    {
        base.Update();

        //if rigid body is not null
        if (rb)
        {
            //update arrow to face the same direction as current velocity
            transform.right = -rb.velocity;
        }
    }


    /// <summary>
    /// Handles logic for when the projectile detects collision with a valid object
    /// </summary>
    /// <param name="collision">the collider of the object that collision has been detected with</param>
    /// <param name="collisionPoint">the exact point at which the projectile hit the collider</param>
    protected override void TriggerHit(Collider2D collision, Vector3 collisionPoint)
    {
        //if arrow an enemy
        if (collision.gameObject.tag == "Enemy")
        {
            //parent arrow to enemy and apply damage to enemy
            transform.parent = collision.gameObject.transform;
            Debug.Log(collision.gameObject);
            collision.gameObject.SendMessage("TakeDamage", 0.5f);
        }
        //if arrow hit an interactable
        else if (collision.gameObject.tag == "Interactable")
        {
            //parent arrow to interactable
            transform.parent = collision.gameObject.transform;
        }

        //destroy arrow's rigid body
        Destroy(rb);

        //snap arrow to point of collision
        transform.position = collisionPoint;

        GetComponent<Animator>().Play("wiggle");
    }
}
