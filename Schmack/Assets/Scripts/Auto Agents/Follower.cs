using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : RoboHordeAgent
{
    public Transform leader;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    { 
        switch (state)
        {
            case State.patrolling:
                ApplyForce(GetSeekForce(leader.position));
                if ((leader.position - transform.position).sqrMagnitude < 5.0f)
                {
                    ApplyForce(GetFleeForce(leader.position));
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
