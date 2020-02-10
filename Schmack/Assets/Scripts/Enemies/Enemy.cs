using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Base Enemy Behavioral Info")]
    [SerializeField] [Range(0.0f, 1.0f)] float maxHealth = 1.0f; //percentage based, 1.0 is highest
    [SerializeField] protected float speed = 2.5f;
    [SerializeField] float playerKnockback = 1000; //how much force does this enemy apply to the player on a hit

    [Header("Base Enemy References")]
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
    bool attacking = false;
    bool seesPlayer = false;
    bool sawLastFrame = false;

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
        seesPlayer = GetSeesPlayer();


        switch (gameState)
        {
            case GameState.patrolling:
                if (!seesPlayer && sawLastFrame)
                {
                    StartCoroutine(CancelAttack());
                }
                Move();
                break;
            case GameState.seesPlayer:
                if (!attacking)
                {
                    attacking = true;
                    StartCoroutine(PrepAttack());
                }
                break;
            case GameState.attacking:
                if (attacking)
                {
                    Debug.Log("top 10 images taken right before disaster");
                    StartCoroutine(Attack());
                    attacking = false;
                }
                break;
            case GameState.dead:
                Destroy(gameObject);
                break;
        }

        if (seesPlayer && gameState != GameState.attacking)
        {
            gameState = GameState.seesPlayer;
        }
        else if(!seesPlayer)
        {
            gameState = GameState.patrolling;
        }

        sawLastFrame = seesPlayer;


        if (health <= 0)
        {
            gameState = GameState.dead;
        }
    }

    protected void TakeDamage(float damage)
    {
        health -= damage;
    }

    protected abstract void Move();

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("whaddup, son?");
        if(collision.gameObject.tag == "Arrow")
        {
            Debug.Log("oof ouch owie my circuits");
            TakeDamage(50);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("oof ouch owie my circuits");
    }

    protected bool GetSeesPlayer()
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

        
        RaycastHit2D rayCast = Physics2D.Raycast(spotlight.transform.position, player.position - spotlight.transform.position, lightToPlayer.magnitude, LayerMask.GetMask("environment"));
        if(rayCast.collider != null)
        {
            Debug.DrawLine(spotlight.transform.position, player.position, Color.white);
            return false;
        }

        Debug.DrawLine(spotlight.transform.position, player.position, Color.red);
        return true;
    }

    protected abstract IEnumerator PrepAttack();

    protected abstract IEnumerator CancelAttack();

    protected abstract IEnumerator Attack();

}
