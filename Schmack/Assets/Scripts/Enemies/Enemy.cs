using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{ 
    [SerializeField] protected float maxHealth = 1.0f;
    public float damage = 1.0f;
    protected float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TakeDamage(DamagePacket packet)
    {
        health -= packet.damage;
        if (health <= 0.0f)
        {
            Die();
        }
    }

    protected abstract void Die();
}
