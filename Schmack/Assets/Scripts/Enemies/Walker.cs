using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [SerializeField] float speed = 5;
    [SerializeField] float waitTime = 1.0f; //how long does he stay at each end of the range before turning around?
    [SerializeField] float visionRange = 1.0f;

    bool isAtEnd = false;
    bool isReversed = true;
    
    float timer = 0.0f;

    Vector2 scale;
    float xScale;

    RaycastHit2D raycastHit;
    //AudioManager audioMan;

    // Start is called before the first frame update
    new void Start()
    {
        //audioMan = AudioManager.instance;
        //if (audioMan == null)
        //{
        //    Debug.LogError("No audiomanager found");
        //}
        base.Start();
        direction = new Vector2(-1.0f, 0.0f);
        timer = waitTime;
        xScale = transform.localScale.x;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        raycastHit = Physics2D.Raycast(transform.position, direction, visionRange, LayerMask.GetMask("environment"));
        Debug.DrawLine(transform.position, (Vector2)transform.position + (direction * visionRange));
        Move();
        scale = transform.localScale;
        scale.x = isReversed ? xScale : -xScale;
        transform.localScale = scale;
        //audioMan.PlaySound("Enemy");
    }


    protected override void Move()
    {
        if (isAtEnd || raycastHit.collider != null)
        {
            rb.velocity = Vector2.zero;
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                direction *= -1;
                isReversed = !isReversed;
                timer = waitTime;
            }
        }
        else
        {
            transform.Translate(direction * speed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAtEnd = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isAtEnd = true;
    }
}
