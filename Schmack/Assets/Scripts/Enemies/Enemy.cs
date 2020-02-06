﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Base enemy info")]
    [SerializeField] float maxHealth = 100.0f;
    [SerializeField] protected float acceleration = 1.0f;
    [SerializeField] protected float maxSpeed = 0.5f;
    [SerializeField] float playerKnockback = 1000;
    [SerializeField] float visualRange = 1.0f;

    [SerializeField] protected Transform player = null;
    [SerializeField] protected UnityEngine.Experimental.Rendering.LWRP.Light2D light = null;

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
        SeesPlayer();
        Move();
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

    protected bool SeesPlayer()
    {
        Vector2 lightToPlayer = player.position - light.transform.position;
        if(lightToPlayer.sqrMagnitude > light.pointLightOuterRadius * light.pointLightOuterRadius)
        {
            Debug.DrawLine(light.transform.position, player.position, Color.white);
            return false;
        }

        float angleToPlayer = Mathf.Atan2(lightToPlayer.x, lightToPlayer.y) * Mathf.Rad2Deg * direction.x;
        if(angleToPlayer < light.pointLightOuterAngle / 2)
        {
            Debug.DrawLine(light.transform.position, player.position, Color.white);
            return false;
        }

        
        RaycastHit2D rayCast = Physics2D.Raycast(light.transform.position, player.position - light.transform.position, lightToPlayer.magnitude, ~LayerMask.GetMask("player"));
        if(rayCast.collider != null)
        {
            Debug.DrawLine(light.transform.position, player.position, Color.white);
            return false;
        }

        Debug.DrawLine(light.transform.position, player.position, Color.red);
        return true;
    }

    protected abstract IEnumerator Attack();
}
