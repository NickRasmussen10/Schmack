﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionReporter : MonoBehaviour
{
    [SerializeField] string colliderID = ""; //unique identification, used by parent game object to tell the difference between multiple collision reporters
    [SerializeField] GameObject reciever = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!reciever)
        {
            reciever = transform.parent.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //assemble a collision packet and send it to the player
        CollisionPacket collisionPacket = new CollisionPacket(true, collision);
        reciever.SendMessage("GetCollisionReport" + colliderID, collisionPacket);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //assemble a collision packet and send it to the player
        CollisionPacket collisionPacket = new CollisionPacket(false, collision);
        reciever.SendMessage("GetCollisionReport" + colliderID, collisionPacket);
    }
}
