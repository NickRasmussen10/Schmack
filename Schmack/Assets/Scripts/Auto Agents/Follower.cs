using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : RoboHordeAgent
{
    public Transform target;
    RoboHordeAgent leader_agent;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //target == leader, assigned by robohorde mananger
        leader_agent = target.gameObject.GetComponent<Leader>();
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
                    ApplyForce(GetFleeForce(target.position));
                }
                else
                {
                    ApplyForce(GetSeekForce(target.position + leader_agent.velocity * 10));
                }
                break;
            case State.attacking:
                ApplyForce(GetSeekForce(target.position));
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
        target = player;
    }

    public void Push(Vector3 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
