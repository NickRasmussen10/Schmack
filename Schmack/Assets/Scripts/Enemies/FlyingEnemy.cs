using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Flying Enemy Info")]
    [SerializeField] float range = 5.0f;
    [SerializeField] float speed = 5.0f;
    [SerializeField] float visionRange = 1.0f;

    float[] limits = new float[2];

    GameObject player;

    enum MovementState
    {
        idle,
        seesPlayer
    }
    MovementState movementState;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        movementState = MovementState.idle;
        limits[0] = transform.position.x - (range / 2);
        limits[1] = transform.position.x + (range / 2);
        direction = new Vector2(1.0f, 0.0f);
    }

    // Update is called once per frame
    new void Update()
    {
        if(transform.position.x < limits[0] || transform.position.x > limits[1])
        {
            direction.x *= -1;
        }
        LookForPlayer();

        base.Update();
    }

    protected override void Move()
    {
        switch (movementState)
        {
            case MovementState.idle:
                transform.Translate(direction * speed * Time.deltaTime); 
                break;
            case MovementState.seesPlayer:
                break;
            default:
                break;
        }
    }

    void LookForPlayer()
    {
        Vector3 h = Vector2.down / Mathf.Sin(visionRange / 2);
        Debug.DrawLine(transform.position, transform.position + (h * 10.0f));
        if(player != null)
        {
            float hypotenuse = Mathf.Abs((transform.position - player.transform.position).sqrMagnitude);
            float deltaX = Mathf.Abs(transform.position.x - player.transform.position.x);
            float angleToPlayer = Mathf.Sin(deltaX / hypotenuse);
            
            if(angleToPlayer < visionRange / 2)
            {
                Debug.Log("oh hello");
            }
        }
    }

    protected override IEnumerator Attack()
    {
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
        }
    }
}
