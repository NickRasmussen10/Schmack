﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth;
    float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Vector2 knockback = (gameObject.transform.position - collision.gameObject.transform.position).normalized * collision.gameObject.GetComponent<Enemy>().GetKnockback();
            gameObject.GetComponent<PlayerMovement>().AddKnockback(knockback, false);
            TakeDamage(25);
        }
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
