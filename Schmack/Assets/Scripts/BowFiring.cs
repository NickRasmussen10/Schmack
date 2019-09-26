using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowFiring : MonoBehaviour
{
    Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        direction = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        direction.x = Input.GetAxis("Horizontal2");
        direction.y = Input.GetAxis("Vertical2");
        Debug.Log(direction);
    }
}
