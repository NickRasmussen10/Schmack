using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionTest : MonoBehaviour
{
    [SerializeField] float explosiveForce = 1.0f;
    Vector3 explosionPoint;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        explosionPoint = GameObject.Find("explosion").transform.position;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("boom");
            rb.AddForce((transform.position - explosionPoint) * explosiveForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-5.0f, 5.0f));
        }
        rb.AddForce((explosionPoint - transform.position) * 5.0f);
    }
}
