using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDetection : MonoBehaviour
{
    public bool isColliding = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isColliding = true;
    }
}
