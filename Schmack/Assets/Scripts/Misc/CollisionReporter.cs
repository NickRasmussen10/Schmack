using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionReporter : MonoBehaviour
{
    [SerializeField] string colliderID = ""; //unique identification, used by parent game object to tell the difference between multiple collision reporters
    [SerializeField] List<GameObject> recievers; //will be set to parent game object if no reciever is given in inspector

    // Start is called before the first frame update
    void Start()
    {
        if (recievers.Count == 0) recievers.Add(null); //this is just so we don't have to constantly add one to the list in the editor
        for(int i = 0; i < recievers.Count; i++)
        {
            if(recievers[i] == null && !recievers.Contains(transform.parent.gameObject))
            {
                recievers[i] = transform.parent.gameObject;
            }
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
        Broadcast(collisionPacket);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //assemble a collision packet and send it to the player
        CollisionPacket collisionPacket = new CollisionPacket(false, collision);
        Broadcast(collisionPacket);
    }

    void Broadcast(CollisionPacket packet)
    {
        foreach (GameObject reciever in recievers)
        {
            reciever.SendMessage("GetCollisionReport" + colliderID, packet);
        }
    }
}
