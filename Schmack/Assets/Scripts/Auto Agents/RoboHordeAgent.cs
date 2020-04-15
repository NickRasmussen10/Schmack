using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboHordeAgent : AutonomousAgent
{
    [SerializeField] protected float lookAheadDistance = 1.0f;
    int pathIndex = 0;

    public enum State
    {
        patrolling, 
        attacking,
        dead
    }
    public State state;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state = State.patrolling;
    }

    // Update is called once per frame
    protected override void Update()
    {
        switch (state)
        {
            case State.patrolling:
                base.Update();
                break;
            case State.attacking:
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
                ApplyForce((GetFleeForce(t.position) / (t.position - transform.position).sqrMagnitude) * 0.1f);
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
