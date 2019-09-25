using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 position;
    private Vector2 direction;
    private Vector2 velocity;
    private Vector2 acceleration;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        direction = Vector2.zero;
        velocity = Vector2.zero;
        acceleration = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector2.zero;
        Gravity();
        float h = Input.GetAxisRaw("Horizontal");
        direction.x = h;

        if (Input.GetButton("Jump"))
        {
            Debug.Log("A is pressed");
        }

        velocity += direction * speed * Time.deltaTime;
        position += velocity;
        transform.position = position;
    }

    void Gravity()
    {
        acceleration.y -= 1;
    }
}
