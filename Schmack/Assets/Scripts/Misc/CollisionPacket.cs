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