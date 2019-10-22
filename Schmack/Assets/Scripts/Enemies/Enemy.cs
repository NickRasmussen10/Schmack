using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] protected float acceleration = 1.0f;
    [SerializeField] protected float maxSpeed = 0.5f;
    protected Rigidbody2D rb;

    float health;
    protected Vector2 direction;

    // Start is called before the first frame update
    protected void Start()
    {
        health = maxHealth;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    protected void TakeDamage(float damage)
    {
        health -= damage;
    }

    protected abstract void Move();
}
