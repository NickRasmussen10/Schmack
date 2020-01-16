using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDetection : MonoBehaviour
{
    public bool isColliding = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        isColliding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isColliding = true;
    }
}
