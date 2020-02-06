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

    // Update is called once per frame
    protected new virtual void Update()
    {
        base.Update();
    }

    protected override void TriggerHit(Collider2D collision, Vector3 collisionPoint)
    {

    }
}
