using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueShot : Projectile
{
    [SerializeField] float speed;

    // Start is called before the first frame update
    protected new virtual void Start()
    {
        base.Start();
        layermask = 1 << 8 | 1 << 9 | 1 << 12;
    }

    public void Fire(Vector3 target)
    {
        rb.AddForce((target - transform.position).normalized * speed, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    protected new virtual void Update()
    {
        base.Update();
    }

    protected override void TriggerHit(Collider2D collision, Vector3 collisionPoint)
    {
        Debug.Log(collision);
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("owie");
            collision.gameObject.GetComponent<Player>().TakeDamage(0.3f);
            StartCoroutine(collision.gameObject.GetComponent<PlayerMovement>().SlowDown(3.0f, 175.0f, 2.5f));
        }
        Destroy(gameObject);
    }
}
