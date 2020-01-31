using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollisionPacket
{
    public bool isColliding;
    public Collider2D collider;

    public CollisionPacket(bool p_isColliding, Collider2D p_Collider)
    {
        isColliding = p_isColliding;
        collider = p_Collider;
    }

    public override string ToString()
    {
        return "{isColliding: " + isColliding + ", collider: " + collider + "}";
    }
}


public class CollisionReporter : MonoBehaviour
{
    [SerializeField] string colliderID = "";
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionPacket collisionPacket = new CollisionPacket(true, collision);
        player.SendMessage("GetCollisionReport" + colliderID, collisionPacket);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollisionPacket collisionPacket = new CollisionPacket(false, collision);
        player.SendMessage("GetCollisionReport" + colliderID, collisionPacket);
    }
}
