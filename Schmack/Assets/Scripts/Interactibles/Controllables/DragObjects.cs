using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObjects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///TODO: this moves the player with the bridge, make it a seperate script preferably on bridge
    //if (collPacket_ground.isColliding)
    //{
    //    Bridge b = raycastHits[0].collider.gameObject.GetComponent<Bridge>();
    //    if (collPacket_ground.collider.gameObject.layer == 12 && b != null && b.direction != 0.0f)
    //    {
    //        transform.Translate(new Vector2(b.openingSpeed * b.direction * Time.deltaTime, 0.0f));
    //    }
    //    else
    //    {
    //        transform.parent = null;
    //    }
    //}
}
