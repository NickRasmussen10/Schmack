﻿using System.Collections;
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
        returning,
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
            case State.returning:
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
                ApplyInnerForce((GetFleeForce(t.position) / (t.position - transform.position).sqrMagnitude) * 0.75f);
            }
        }
    }

    public override void Die()
    {
        state = State.dead;
        GetComponent<BoxCollider2D>().size = Vector2.one;
        gameObject.layer = 17;
        base.Die();
    }

    public override void Die(Vector3 force)
    {
        state = State.dead;
        GetComponent<BoxCollider2D>().size = Vector2.one;
        gameObject.layer = 17;
        base.Die(force);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("environment"))
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
