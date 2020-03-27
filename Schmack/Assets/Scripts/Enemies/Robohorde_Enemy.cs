using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robohorde_Enemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Die()
    {
        GetComponent<RoboHordeAgent>().Die();
    }

    public override void TakeDamage(DamagePacket packet)
    {
        if (packet.isPowerShot)
        {
            DamagePacket newPacket;
            newPacket.damage = packet.damage;
            newPacket.isPowerShot = false;
            Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 10.0f, LayerMask.GetMask("enemies"));
            foreach (Collider2D enemy in nearby)
            {
                enemy.SendMessage("TakeDamage", newPacket);
            }
        }
        base.TakeDamage(packet);
    }
}
