using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobohordeManager : MonoBehaviour
{
    [SerializeField] float visionRange = 10.0f;
    [SerializeField] float attackRate = 1.0f;

    Transform leaderTransform;
    RoboHordeAgent leaderAgent;
    List<Transform> followerTransforms = new List<Transform>();
    List<RoboHordeAgent> followerAgents = new List<RoboHordeAgent>();

    Transform player;

    Coroutine c_attack;

    private void Awake()
    {
        RoboHordeAgent[] followers = GetComponentsInChildren<Follower>();
        leaderAgent = GetComponentInChildren<Leader>();
        leaderTransform = leaderAgent.gameObject.transform;

        foreach (Follower follower in followers)
        {
            followerTransforms.Add(follower.gameObject.transform);
            followerAgents.Add(follower);
            follower.target = leaderTransform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (RoboHordeAgent agent in followerAgents)
        {
            agent.ApplyFlocking(followerTransforms);
        }

        if((player.position - leaderTransform.position).sqrMagnitude < visionRange * visionRange)
        {
            if(c_attack == null)
            {
                c_attack = StartCoroutine(Attack());
            }
        }
        else if(c_attack != null)
        {
            StopCoroutine(c_attack);
            c_attack = null;
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            Follower[] furthest = new Follower[3];
            float[] distances = new float[3];
            for(int i = 0; i < distances.Length; i++) { distances[i] = float.MinValue; }

            foreach (Transform follower in followerTransforms)
            {
                float distance = (leaderTransform.position - follower.position).sqrMagnitude;
                if(distance > distances[0])
                {
                    furthest[2] = furthest[1];
                    furthest[1] = furthest[0];
                    furthest[0] = follower.GetComponent<Follower>();

                    distances[2] = distances[1];
                    distances[1] = distances[0];
                    distances[0] = distance;
                }
                else if(distance > distances[1])
                {
                    furthest[2] = furthest[1];
                    furthest[1] = follower.GetComponent<Follower>();

                    distances[2] = distances[1];
                    distances[1] = distance;
                }
                else if(distance > distances[2])
                {
                    furthest[2] = follower.GetComponent<Follower>();
                    distances[2] = distance;
                }
            }

            Debug.Log(furthest[0]);

            foreach (Follower follower in furthest)
            {
                follower.Attack(player);
                follower.Push((leaderTransform.position - follower.transform.position).normalized * 10);
            }

            yield return new WaitForSeconds(attackRate);
        }
    }
}
