using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboHordeAgent : AutonomousAgent
{
    [SerializeField] List<Transform> pathway = new List<Transform>();
    [SerializeField] Transform leader;
    [SerializeField] float lookAheadDistance = 1.0f;
    [SerializeField] Transform[] targets;
    Transform target;
    int pathIndex = 0;

    enum BotType
    {
        leader,
        follower
    }
    [SerializeField] BotType role;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if(role == BotType.follower)
        {
            target = targets[Random.Range(0, 2)];
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
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
                ApplyForce(GetSeekForce(target.position));
                break;
            default:
                break;
        }

        base.Update();
    }
}
