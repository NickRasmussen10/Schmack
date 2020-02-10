using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollisionPacket
{
    public bool isColliding;
    public Collider2D collider; //object with which collision has been detected

    /// <summary>
    /// Collision packet contructor
    /// </summary>
    /// <param name="p_isColliding">true if this collision packet is reporting a collision</param>
    /// <param name="p_Collider">the collider of the object this collision packet is reporting on</param>
    public CollisionPacket(bool p_isColliding, Collider2D p_Collider)
    {
        isColliding = p_isColliding;
        collider = p_Collider;
    }

    /// <summary>
    /// Outputs this collision packet's info to a string
    /// </summary>
    /// <returns>string of info on this Collision Packet</returns>
    public override string ToString()
    {
        return "{isColliding: " + isColliding + ", collider: " + collider + "}";
    }
}


public class CollisionReporter : MonoBehaviour
{
    [SerializeField] string colliderID = ""; //unique identification, used by parent game object to tell the difference between multiple collision reporters
    GameObject parent; //parent object 

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //assemble a collision packet and send it to the player
        CollisionPacket collisionPacket = new CollisionPacket(true, collision);
        parent.SendMessage("GetCollisionReport" + colliderID, collisionPacket);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //assemble a collision packet and send it to the player
        CollisionPacket collisionPacket = new CollisionPacket(false, collision);
        parent.SendMessage("GetCollisionReport" + colliderID, collisionPacket);
    }
}
