using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [SerializeField] float speed = 0.05f;
    [SerializeField] float waitTime = 1.0f; //how long does he stay at each end of the range before turning around?
    [SerializeField] float visionRange = 1.0f;

    EndDetection endDetector = null;
    bool isReversed = true;
    
    float timer = 0.0f;
    float detectionRange = 0.5f; //how close does he need to get before he arrives at either end of the range
    Vector2 scale;

    RaycastHit2D raycastHit;
    bool seesPlayer = false;

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
        endDetector = gameObject.GetComponentInChildren<EndDetection>();
        direction = new Vector2(-1.0f, 0.0f);
        timer = waitTime;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        raycastHit = Physics2D.Raycast(transform.position, direction, visionRange, LayerMask.GetMask("player"));
        if(raycastHit.collider != null)
        {
            //target = raycastHit.collider.gameObject;
            seesPlayer = true;
        }
        Debug.DrawLine(transform.position, (Vector2)transform.position + (direction * visionRange));
        Move();
        Vector3 scale = transform.localScale;
        scale.x = isReversed ? 1 : -1;
        transform.localScale = scale;
        //audioMan.PlaySound("Enemy");
    }


    protected override void Move()
    {
        if (endDetector.isColliding)
        {
            //Debug.Log("moving");
            rb.AddForce(direction * speed);
        }
        else
        {
            rb.velocity = Vector2.zero;
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                Debug.Log("timer's up");
                direction *= -1;
                isReversed = !isReversed;
                timer = waitTime;
            }
        }
    }
}
