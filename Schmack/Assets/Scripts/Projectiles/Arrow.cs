using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    protected bool hasHit = false;

    // Start is called before the first frame update
    protected new virtual void Start()
    {
        base.Start();
        transform.right = -rb.velocity;
    }

    // Update is called once per frame
    protected new virtual void Update()
    {
        base.Update();

        //if (hasHit && rb != null)
        //{
        //    Destroy(rb);
        //    Destroy(collider);
        //}
        if (rb != null)
        {
            transform.right = -rb.velocity;
        }
    }

    protected override void TriggerHit(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Arrow")
        {
            hasHit = true;
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Interactable")
                transform.parent = collision.gameObject.transform;
            Destroy(rb);
            Destroy(boxCollider);
        }
    }

    public void AddForce(Vector2 force)
    {
        rb.AddForce(force);
    }

    public void AddForce(float x, float y)
    {
        AddForce(new Vector2(x, y));
    }
}
