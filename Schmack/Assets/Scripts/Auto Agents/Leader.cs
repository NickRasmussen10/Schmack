﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : RoboHordeAgent
{
    [SerializeField] List<Transform> pathway = new List<Transform>();
    int targetIndex = 0;


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
                if (pathway[targetIndex] != null)
                {
                    ApplyInnerForce(GetSeekForce(pathway[targetIndex].position));

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
