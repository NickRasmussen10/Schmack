using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] float waitTime = 1.0f; //how long does he stay at each end of the range before turning around?
    [SerializeField] float visionRange = 1.0f;
    float timer = 0.0f;
    float detectionRange = 0.5f; //how close does he need to get before he arrives at either end of the range
    GameObject target;
    Vector2 scale;

    RaycastHit2D raycastHit;
    bool seesPlayer = false;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        target = start;
        timer = waitTime;
        direction = (target.transform.position - gameObject.transform.position).normalized;
        direction.y = 0;
        scale = transform.localScale;
        scale.x = target == start ? scale.x : -scale.x;
        transform.localScale = scale;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        raycastHit = Physics2D.Raycast(transform.position, direction, visionRange, LayerMask.GetMask("player"));
        if(raycastHit.collider != null)
        {
            target = raycastHit.collider.gameObject;
            seesPlayer = true;
        }
        Debug.DrawLine(transform.position, (Vector2)transform.position + (direction * visionRange));
        Move();
    }


    protected override void Move()
    {
        if (!seesPlayer)
        {
            if (Mathf.Pow(target.transform.position.x - gameObject.transform.position.x, 2) < detectionRange * detectionRange)
            {
                //wait and swap direction
                timer -= Time.deltaTime;
                rb.velocity = Vector2.zero;
                if (timer <= 0)
                {
                    //swap targets
                    target = target == start ? end : start;

                    direction = (target.transform.position - gameObject.transform.position).normalized;
                    direction.y = 0;
                    scale.x *= -1;
                    transform.localScale = scale;

                    timer = waitTime;
                }
            }
            else
            {
                //seek target
                rb.AddForce(direction * acceleration * (Mathf.Abs(target.transform.position.x - gameObject.transform.position.x)));
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
            }
        }
        else
        {
            Debug.Log("whaddup, son?");
            //seek player
            direction = (target.transform.position - transform.position).normalized;
            direction.y = 0;
            if((direction.x > 0 && scale.x > 0) || (direction.x < 0 && scale.x < 0))
            {
                scale.x *= -1;
                transform.localScale = scale;
            }
            transform.localScale = scale;
            rb.AddForce(direction * acceleration * (Mathf.Abs(target.transform.position.x - gameObject.transform.position.x)));
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        }
    }
}
