using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobohordeManager : MonoBehaviour
{
    RoboHordeAgent leader;
    List<Transform> followerTransforms = new List<Transform>();
    List<RoboHordeAgent> followerAgents = new List<RoboHordeAgent>();

    // Start is called before the first frame update
    void Start()
    {
        RoboHordeAgent[] horde = GetComponentsInChildren<RoboHordeAgent>();
        foreach (RoboHordeAgent agent in horde)
        {
            if (agent.role == RoboHordeAgent.BotType.leader) leader = agent;
            else
            {
                followerTransforms.Add(agent.gameObject.transform);
                followerAgents.Add(agent);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (RoboHordeAgent agent in followerAgents)
        {
            agent.ApplyFlocking(followerTransforms);
        }
    }
}
