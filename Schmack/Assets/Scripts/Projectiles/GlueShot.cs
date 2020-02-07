using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlueShot : Projectile
{
    // Start is called before the first frame update
    protected new virtual void Start()
    {
        base.Start();
    }

    public void Fire(Vector3 start, Vector3 target)
    {
        rb.AddForce((target - start).normalized * 10.0f, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    protected new virtual void Update()
    {
        base.Update();
    }

    protected override void TriggerHit(Collider2D collision, Vector3 collisionPoint)
    {
        Destroy(gameObject);
    }
}
