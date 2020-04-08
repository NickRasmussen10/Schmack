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
        RoboHordeAgent[] followers = GetComponentsInChildren<Follower>();
        leader = GetComponentInChildren<Leader>();

        foreach (Follower follower in followers)
        {
            followerTransforms.Add(follower.gameObject.transform);
            followerAgents.Add(follower);
            follower.leader = leader.transform;
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
