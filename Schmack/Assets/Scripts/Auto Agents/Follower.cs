using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : RoboHordeAgent
{
    public Transform leader;
    RoboHordeAgent leader_agent;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        leader_agent = leader.gameObject.GetComponent<Leader>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (leader_agent.state == State.dead && state != State.dead) Die();
        
        switch (state)
        {
            case State.patrolling:
                if ((leader.position - transform.position).sqrMagnitude < lookAheadDistance)
                {
                    ApplyForce(GetFleeForce(leader.position));
                }
                else
                {
                    ApplyForce(GetSeekForce(leader.position + leader.gameObject.GetComponent<AutonomousAgent>().velocity * 10));
                }
                break;
            case State.dead:
                break;
            default:
                break;
        }

        base.Update();
    }
}
