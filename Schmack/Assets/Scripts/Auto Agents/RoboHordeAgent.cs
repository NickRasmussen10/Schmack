using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboHordeAgent : AutonomousAgent
{
    [SerializeField] List<Transform> pathway = new List<Transform>();
    [SerializeField] Transform leader;
    [SerializeField] float lookAheadDistance = 1.0f;
    [SerializeField] Transform[] targets;
    [SerializeField] Transform target;
    int pathIndex = 0;

    public enum BotType
    {
        leader,
        follower
    }
    public BotType role;

    enum State
    {
        patrolling, 
        dead
    }
    State state;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(role == BotType.follower)
        {
            target = targets[Random.Range(0, targets.Length)];
            //target = leader;
        }
        state = State.patrolling;
    }

    // Update is called once per frame
    protected override void Update()
    {
        switch (state)
        {
            case State.patrolling:
                switch (role)
                {
                    case BotType.leader:
                        ApplyForce(GetSeekForce(pathway[pathIndex].position));

                        if ((pathway[pathIndex].position - transform.position).sqrMagnitude < lookAheadDistance)
                        {
                            if (pathIndex == pathway.Count - 1)
                            {
                                pathIndex = 0;
                            }
                            else
                            {
                                pathIndex++;
                            }
                        }
                        break;
                    case BotType.follower:
                        //if((transform.position - leader.position).sqrMagnitude < 10.0f)
                        //{
                        //    ApplyForce(GetFleeForce(leader.position));
                        //}
                        //else
                        //{
                        //    ApplyForce(GetSeekForce(leader.position) * 1.25f);
                        //}
                        ApplyForce(GetSeekForce(target.position));
                        if((leader.position - transform.position).sqrMagnitude < 5.0f)
                        {
                            ApplyForce(GetFleeForce(leader.position));
                        }
                        break;
                    default:
                        break;
                }
                base.Update();
                break;
            case State.dead:
                break;
            default:
                break;
        }
    }

    public void ApplyFlocking(List<Transform> horde)
    {
        foreach (Transform t in horde)
        {
            if (t != transform)
            {
                ApplyForce((GetFleeForce(t.position) / (t.position - transform.position).sqrMagnitude) * 0.25f);
            }
        }
    }

    public void Die()
    {
        state = State.dead;
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        rb.gravityScale = 1.0f;
    }
}
