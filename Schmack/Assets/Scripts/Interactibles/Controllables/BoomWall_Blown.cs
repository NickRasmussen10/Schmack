using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomWall_Blown : MonoBehaviour
{
    Rigidbody2D[] arrRB;

    float boomSize = 50.0f;


    // Start is called before the first frame update
    void Start()
    {
        arrRB = GetComponentsInChildren<Rigidbody2D>();

        if (Input.GetButtonDown("SwapWeapon"))
        {
            foreach (Rigidbody2D rigidbody in arrRB)
            {
                Debug.Log(rigidbody);
                rigidbody.AddForce(new Vector2(Random.Range(-boomSize, boomSize), Random.Range(-boomSize, boomSize)), ForceMode2D.Impulse);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
