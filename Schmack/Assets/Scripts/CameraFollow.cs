using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Cori Mori
//10-15-19
//Script that allows the camera to follow the player through the level
public class CameraFollow : MonoBehaviour
{
    public GameObject player;       // Reference to the player that the camera will follow

    private Vector3 offset;         // Offset distance between the player and the camera

    // Start is called before the first frame update
    void Start()
    {
        //Calculte and stroe the offset values by getting the distance between the player's position and the camera's position
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        //Set the position of the camera's transform to be the same as the player's plus the offset
        transform.position = player.transform.position + offset;
    }
}
