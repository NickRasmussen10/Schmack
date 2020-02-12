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
    bool seesPlayer = false;

    protected Coroutine attack;

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
        //store if enemy sees the player
        seesPlayer = GetSeesPlayer();


        switch (gameState)
        {
            case GameState.patrolling:
                //if this is the first frame the enemy has not seen the player
                if (!seesPlayer && attack != null)
                {
                    StartCoroutine(CancelAttack());
                    attack = null;
                }
                Move();
                break;
            case GameState.seesPlayer:
                //if this is the first frame the enemy is attacking
                if (attack == null)
                {
                    attack = StartCoroutine(PrepAttack());
                }
                break;
            case GameState.attacking:
                //if this is the first frame the enemy is attacking and the game state is attacking
                if (attack == null)
                {
                    Debug.Log("top 10 images taken right before disaster");
                    attack = StartCoroutine(Attack());
                }
                break;
            case GameState.dead:
                Destroy(gameObject);
                break;
        }

        //if the enemy sees the player and it is not in attacking state
        if (seesPlayer && gameState != GameState.attacking)
        {
            gameState = GameState.seesPlayer;
        }

        //if the enemy does not see the player
        else if(!seesPlayer)
        {
            gameState = GameState.patrolling;
        }


        if (health <= 0)
        {
            gameState = GameState.dead;
        }
    }


    /// <summary>
    /// Handles the enemy recieving damage
    /// </summary>
    /// <param name="damage"></param>
    protected void TakeDamage(float damage)
    {
        health -= damage;
    }

    /// <summary>
    /// Handles the enemy's movement patterns
    /// </summary>
    protected abstract void Move();


    /// <summary>
    /// returns true is the enemy can see the player, false otherwise
    /// </summary>
    /// <returns></returns>
    protected bool GetSeesPlayer()
    {
        //find the vector from the enemy's light to the player
        Vector2 lightToPlayer = player.position - spotlight.transform.position;

        //if light-to-player vector's magnitude is larger than the light's range, enemy cannot see player
        if(lightToPlayer.sqrMagnitude > (spotlight.pointLightOuterRadius * spotlight.pointLightOuterRadius) * 0.6f)
        {
            return false;
        }



        //find angle between enemy's light and player
        float angleToPlayer = Mathf.Atan2(lightToPlayer.x, lightToPlayer.y) * Mathf.Rad2Deg * direction.x;

        //if player is too far to the side of the light, enemy cannot see player
        if(angleToPlayer < (spotlight.pointLightOuterAngle / 2) * 0.8f)
        {
            return false;
        }

        
        //cast a ray from the enemy's light to the player
        RaycastHit2D rayCast = Physics2D.Raycast(spotlight.transform.position, player.position - spotlight.transform.position, lightToPlayer.magnitude, LayerMask.GetMask("environment"));

        //if the ray hits anything, enemy cannot see player
        if (rayCast.collider != null)
        {
            return false;
        }

        //if all checks passed, enemy can see player
        return true;
    }

    /// <summary>
    /// Handles "calculation delay time" and bringing the enemy to an attack state
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator PrepAttack();

    /// <summary>
    /// Brings enemy back to patrolling state from attack
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator CancelAttack();

    /// <summary>
    /// Handles enemy attacking player
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator Attack();

}
