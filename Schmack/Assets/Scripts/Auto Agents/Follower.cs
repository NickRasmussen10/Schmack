using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : RoboHordeAgent
{
    [SerializeField] float attackRange = 1.0f;
    public Transform target;
    RoboHordeAgent leader_agent;

    BoxCollider2D boxCollider;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //target == leader, assigned by robohorde mananger
        leader_agent = target.gameObject.GetComponent<Leader>();
        boxCollider = GetComponent<BoxCollider2D>();
        state = State.patrolling;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (leader_agent.state == State.dead && state != State.dead) Die();
        
        switch (state)
        {
            case State.patrolling:
                if ((target.position - transform.position).sqrMagnitude < lookAheadDistance)
                {
                    ApplyInnerForce(GetFleeForce(target.position));
                }
                else
                {
                    ApplyInnerForce(GetSeekForce(target.position + leader_agent.innerVelocity * 10));
                }
                break;
            case State.attacking:
                if ((target.position - transform.position).sqrMagnitude > attackRange * attackRange) { Debug.Log("no wait go back"); ReturnToLeader(); }
                ApplyInnerForce(GetSeekForce(target.position));
                
                break;
            case State.returning:
                ApplyInnerForce(GetSeekForce(target.position));
                if((transform.position - target.position).sqrMagnitude < 3)
                {
                    state = State.patrolling;
                }
                break;
            case State.dead:
                break;
            default:
                break;
        }

        base.Update();
    }

    public void Attack(Transform player)
    {
        state = State.attacking;
        boxCollider.size = Vector2.one * 2;
        target = player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && state == State.attacking)
        {
            collision.gameObject.SendMessage("TakeDamage", 0.05f);
            ReturnToLeader();
        }
    }

    public void ReturnToLeader()
    {
        target = leader_agent.gameObject.transform;
        boxCollider.size = Vector2.one;
        state = State.returning;
    }

    public void Push(Vector3 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public bool IsAttacking() { return state == State.attacking; }
    public bool IsDead() { return state == State.dead; }
    public bool IsPatrolling() { return state == State.patrolling; }
}
