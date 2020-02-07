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

    [SerializeField] protected Transform player = null;
    [SerializeField] protected UnityEngine.Experimental.Rendering.LWRP.Light2D spotlight = null;

    protected enum GameState
    {
        patrolling,
        seesPlayer,
        attacking,
        dead
    }

    protected GameState gameState;

    public float GetKnockback() { return playerKnockback; }
    protected Rigidbody2D rb;

    float health;
    protected Vector2 direction;

    // Start is called before the first frame update
    protected void Start()
    {
        health = maxHealth;
        rb = gameObject.GetComponent<Rigidbody2D>();
        gameState = GameState.patrolling;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (SeesPlayer())
        {
            gameState = GameState.seesPlayer;
        }
        else
        {
            gameState = GameState.patrolling;
        }

        

        if(health <= 0)
        {
            gameState = GameState.dead;
        }


        switch (gameState)
        {
            case GameState.patrolling:
                StartCoroutine(CancelAttack());
                Move();
                break;
            case GameState.seesPlayer:
                StartCoroutine(PrepAttack());
                break;
            case GameState.attacking:
                break;
            case GameState.dead:
                Destroy(gameObject);
                break;
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
        Vector2 lightToPlayer = player.position - spotlight.transform.position;
        if(lightToPlayer.sqrMagnitude > spotlight.pointLightOuterRadius * spotlight.pointLightOuterRadius)
        {
            Debug.DrawLine(spotlight.transform.position, player.position, Color.white);
            return false;
        }

        float angleToPlayer = Mathf.Atan2(lightToPlayer.x, lightToPlayer.y) * Mathf.Rad2Deg * direction.x;
        if(angleToPlayer < spotlight.pointLightOuterAngle / 2)
        {
            Debug.DrawLine(spotlight.transform.position, player.position, Color.white);
            return false;
        }

        
        RaycastHit2D rayCast = Physics2D.Raycast(spotlight.transform.position, player.position - spotlight.transform.position, lightToPlayer.magnitude, ~LayerMask.GetMask("player"));
        if(rayCast.collider != null)
        {
            Debug.DrawLine(spotlight.transform.position, player.position, Color.white);
            return false;
        }

        Debug.DrawLine(spotlight.transform.position, player.position, Color.red);
        Debug.Log("whaddup, son?");
        return true;
    }

    protected abstract IEnumerator PrepAttack();

    protected abstract IEnumerator CancelAttack();

    protected abstract IEnumerator Attack();

}
