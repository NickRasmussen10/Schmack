using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : Controller
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 9)
        {
            Activate();
            Vector3 scale = transform.localScale;
            scale.y /= 2;
            transform.localScale = scale;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 9)
        {
            Activate();
            Vector3 scale = transform.localScale;
            scale.y *= 2;
            transform.localScale = scale;
        }
    }
}
