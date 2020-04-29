using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobohordeManager : MonoBehaviour
{
    [SerializeField] float visionRange = 10.0f;
    [SerializeField] float attackRate = 1.0f;
    public float hordeSize = 20;
    [SerializeField] GameObject pref_leader;
    [SerializeField] GameObject pref_follower;

    bool attackEnabled = false;
    public void EnableAttack() { attackEnabled = true; }

    Transform leaderTransform;
    Leader leaderAgent;
    List<Transform> followerTransforms = new List<Transform>();
    List<RoboHordeAgent> followerAgents = new List<RoboHordeAgent>();

    Transform player;

    Coroutine c_attack;

    RaycastHit2D raycastHit;

    private void Awake()
    {
        //RoboHordeAgent[] followers = GetComponentsInChildren<Follower>();
        leaderAgent = GetComponentInChildren<Leader>();
        leaderTransform = leaderAgent.gameObject.transform;

        //foreach (Follower follower in followers)
        //{
        //    followerTransforms.Add(follower.gameObject.transform);
        //    followerAgents.Add(follower);
        //    follower.target = leaderTransform;
        //}
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
        if (leaderTransform != null)
        {
            if (attackEnabled && followerAgents.Count != 0 && (player.position - leaderTransform.position).sqrMagnitude < visionRange * visionRange)
            {
                raycastHit = Physics2D.Raycast(leaderTransform.position, (player.position - leaderTransform.position).normalized, Vector3.Distance(player.position, leaderTransform.position), LayerMask.GetMask("environment"));
                if (raycastHit.collider == null && c_attack == null)
                {
                    c_attack = StartCoroutine(Attack());
                }
            }
            else if (c_attack != null)
            {
                StopCoroutine(c_attack);
                c_attack = null;
            }
        }
    }

    public void SetPath(Transform[] path)
    {
        leaderAgent.SetPath(path);
    }

    public void SpawnLeader(Vector3 force)
    {
        if (leaderAgent == null)
        {
            leaderAgent = Instantiate(pref_leader, transform).GetComponent<Leader>();
            leaderTransform = leaderAgent.gameObject.transform;
        }
        leaderAgent.ApplyForce(force);

    }

    public void SpawnFollower(Vector3 force)
    {
        Follower follower = Instantiate(pref_follower, transform).GetComponent<Follower>();
        follower.target = leaderTransform;
        followerAgents.Add(follower);
        followerTransforms.Add(follower.gameObject.transform);
        follower.ApplyForce(force);
    }

    IEnumerator Attack()
    {
        while (true)
        {
            Follower[] furthest = new Follower[3];
            float[] distances = new float[3];
            for (int i = 0; i < distances.Length; i++) { distances[i] = float.MinValue; }

            foreach (Follower follower in followerAgents)
            {
                if (follower.IsPatrolling())
                {
                    float distance = (leaderTransform.position - follower.gameObject.transform.position).sqrMagnitude;
                    if (distance > distances[0])
                    {
                        furthest[2] = furthest[1];
                        furthest[1] = furthest[0];
                        furthest[0] = follower;

                        distances[2] = distances[1];
                        distances[1] = distances[0];
                        distances[0] = distance;
                    }
                    else if (distance > distances[1])
                    {
                        furthest[2] = furthest[1];
                        furthest[1] = follower;

                        distances[2] = distances[1];
                        distances[1] = distance;
                    }
                    else if (distance > distances[2])
                    {
                        furthest[2] = follower;
                        distances[2] = distance;
                    }
                }
            }
            
            foreach (Follower follower in furthest)
            {
                follower.Attack(player);
                follower.Push((leaderTransform.position - follower.transform.position).normalized * 10);
            }

            yield return new WaitForSeconds(attackRate);
        }
    }
}
