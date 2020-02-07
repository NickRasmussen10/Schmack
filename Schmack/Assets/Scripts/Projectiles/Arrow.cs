using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{

    [SerializeField] TrailRenderer trail = null;

    protected bool hasHit = false;

    // Start is called before the first frame update
    protected new virtual void Start()
    {
        base.Start();
        transform.right = -rb.velocity;
        Instantiate(trail, transform);
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

    protected override void TriggerHit(Collider2D collision, Vector3 collisionPoint)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.tag != "Arrow")
        {
            hasHit = true;
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Interactable")
                transform.parent = collision.gameObject.transform;
            Destroy(rb);
            Destroy(collider);

            //collisionPoint.z = 1;
            transform.position = collisionPoint;
            GetComponent<Animator>().Play("wiggle");
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
