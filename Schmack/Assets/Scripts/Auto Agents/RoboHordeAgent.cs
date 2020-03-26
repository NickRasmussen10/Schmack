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

    float health = 0.5f;

    enum BotType
    {
        leader,
        follower
    }
    [SerializeField] BotType role;

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
            target = targets[Random.Range(0, 3)];
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
                        ApplyForce(GetSeekForce(target.position));
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

    public void TakeDamage(DamagePacket packet)
    {
        health -= packet.damage;
        if (packet.isPowerShot)
        {
            DamagePacket newPacket;
            newPacket.damage = packet.damage;
            newPacket.isPowerShot = false;
            Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 10.0f, LayerMask.GetMask("enemies"));
            foreach (Collider2D enemy in nearby)
            {
                enemy.SendMessage("TakeDamage", newPacket);
            }
        }
        if (health <= 0.0f)
        {
            Die();
        }
    }

    void Die()
    {
        state = State.dead;
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        rb.gravityScale = 1.0f;
    }
}
