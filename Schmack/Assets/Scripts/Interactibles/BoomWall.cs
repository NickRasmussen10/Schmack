using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomWall : MonoBehaviour
{
    [SerializeField] GameObject pref_boomwall_blown = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BombArrow")
        {
            Instantiate(pref_boomwall_blown, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
