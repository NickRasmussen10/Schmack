using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboHordeAgent : AutonomousAgent
{
    [SerializeField] List<Transform> pathway = new List<Transform>();
    [SerializeField] Transform leader;
    [SerializeField] float lookAheadDistance = 1.0f;
    int targetIndex = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        switch (role)
        {
            case BotType.leader:
                transform.Translate(Seek(pathway[targetIndex].position));

                if ((pathway[targetIndex].position - transform.position).sqrMagnitude < lookAheadDistance)
                {
                    if (targetIndex == pathway.Count - 1)
                    {
                        targetIndex = 0;
                    }
                    else
                    {
                        targetIndex++;
                    }
                }
                break;
            case BotType.follower:
                transform.Translate(Seek(leader.position));
                break;
            default:
                break;
        }
    }
}
