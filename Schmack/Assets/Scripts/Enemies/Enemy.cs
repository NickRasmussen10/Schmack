using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamagePacket
{
    public float damage;
    public bool isPowerShot;
}

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

    void TakeDamage(float pDamage)
    {
        health -= damage;
    }

    protected abstract void Die();
}
