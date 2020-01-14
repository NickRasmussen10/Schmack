using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] protected float acceleration = 1.0f;
    [SerializeField] protected float maxSpeed = 0.5f;
    [SerializeField] float playerKnockback = 1000;
    public float GetKnockback() { return playerKnockback; }
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
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected void TakeDamage(float damage)
    {
        health -= damage;
    }

    protected abstract void Move();

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Arrow")
        {
            TakeDamage(50);
        }
    }
}
