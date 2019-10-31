using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineBounce : MonoBehaviour
{
    [SerializeField] float bounceSize = 0.01f;
    float timer = 0.0f;
    Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        position.y += Mathf.Sin(timer) * bounceSize;
        transform.position = position;
    }
}
