using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    bool hasHit = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (hasHit && rb)
        {
            transform.right = rb.velocity;
            Destroy(rb);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            hasHit = true;
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
