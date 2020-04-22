using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : RoboHordeAgent
{
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
        //StartCoroutine(PhysicsTest());
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
                if (boxCollider.size != Vector2.one) boxCollider.size = Vector2.one;
                else
                {
                    ApplyInnerForce(GetSeekForce(target.position + leader_agent.innerVelocity * 10));
                }
                break;
            case State.attacking:
                ApplyInnerForce(GetSeekForce(target.position));
                if (boxCollider.size != Vector2.one * 2) boxCollider.size = Vector2.one * 2;
                break;
            case State.dead:
                break;
            default:
                break;
        }

        base.Update();
    }

    //had to write a coroutine to test new physics because unity inputs are like hard now
    IEnumerator PhysicsTest()
    {
        ApplyForce(new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0.0f));
        while (true)
        {
            yield return new WaitForSeconds(30.0f);
            ApplyForce(new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0.0f));
        }
    }

    public void Attack(Transform player)
    {
        state = State.attacking;
        target = player;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && state == State.attacking)
        {
            collision.gameObject.SendMessage("TakeDamage", 0.05f);
            target = leader_agent.gameObject.transform;
            state = State.patrolling;
        }
    }

    public void Push(Vector3 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public bool IsAttacking() { return state == State.attacking; }
    public bool IsDead() { return state == State.dead; }
    public bool IsPatrolling() { return state == State.patrolling; }
}
