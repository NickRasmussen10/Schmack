using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        


        position += velocity;
        transform.position = position;
    }


    Vector3 AddForce(Vector3 force)
    {
        
    }
}
