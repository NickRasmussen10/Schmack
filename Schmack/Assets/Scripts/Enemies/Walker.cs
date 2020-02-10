using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [Header("Walking Enemy Info")]
    [SerializeField] float speed = 5;
    [SerializeField] float turnTime = 1.0f; //how long does he stay at each end of the range before turning around?
    [SerializeField] float reactionTime = 1.0f; //how long in seconds does it take for the enemy to attack the player?
    [SerializeField] float shotCooldown = 1.0f; //how long in seconds does it take for the enemy to fire another shot?
    [SerializeField] Transform rotator = null; //"head" that rotates to look at player / facing direction
    [SerializeField] GameObject pref_GlueShot = null; 

    [SerializeField] BoxCollider2D groundDetector = null;
    [SerializeField] BoxCollider2D wallDetector = null;

    bool isAtEnd = false;
    bool turning = false;
    
    float timer = 0.0f;

    Vector2 scale;
    float xScale;

    RaycastHit2D raycastHit;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        //start facing left
        direction = new Vector2(-1.0f, 0.0f);
        xScale = transform.localScale.x;
        rotator.right = Vector3.left;

        //update detectors to be on correct side of enemy
        Vector2 offset = groundDetector.offset;
        offset.x = -GetComponent<BoxCollider2D>().size.x / 2;
        groundDetector.offset = offset;

        //start with full turn timer
        timer = turnTime;
    }

    // Update is called once per frame
    new void Update()
    {
        //raycast in front of enemy for wall detection      NOTE: obsolete, replace with wall detection box collider
        raycastHit = Physics2D.Raycast(transform.position + new Vector3(0.0f, 0.5f, 0.0f), direction, 3.0f, LayerMask.GetMask("environment"));

        base.Update();
    }


    /// <summary>
    /// Handles the enemy's movement patterns
    /// </summary>
    protected override void Move()
    {
        //if te end of the platform has been detected (edge or wall)
        if(isAtEnd || raycastHit.collider != null)
        {
            //turn around in given direcion
            if (!turning)
            {
                StartCoroutine(SwapDirections());
            }
        }
        else
        {
            //move forward
            transform.Translate(direction * speed);
        }
    }


    /// <summary>
    /// Handles changing directions by rotating the head, updating direction
    /// </summary>
    /// <returns></returns>
    IEnumerator SwapDirections()
    {
        //mark coroutine as running
        turning = true;

        //wait for turnTime seconds
        yield return new WaitForSeconds(turnTime);

        //determine direction of turn
        float lerpVal = direction.x < 0 ? 0.0f : 1.0f;
        Vector3 right = Vector3.zero;

        
        //lerp turn left to right
        if(lerpVal == 0.0f)
        {
            float rotation = 0.0f;
            while (lerpVal < 1.0f)
            {
                lerpVal += Time.deltaTime * 3.0f;
                if (lerpVal > 1.0f) lerpVal = 1.0f;

                rotation = Mathf.Lerp(180.0f, 0.0f, lerpVal);
                rotator.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
                yield return null;
            }
        }

        //lerp turn right to left
        else
        {
            float rotation = 0.0f;
            while (lerpVal > 0.0f)
            {
                lerpVal -= Time.deltaTime * 3.0f;
                if (lerpVal < 0.0f) lerpVal = 0.0f;

                rotation = Mathf.Lerp(180.0f, 0.0f, lerpVal);
                rotator.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
                yield return null;
            }
        }
        turning = false;
        Vector2 offset = groundDetector.offset;
        offset.x *= -1;
        groundDetector.offset = offset;
        direction.x *= -1;
    }

    protected override IEnumerator PrepAttack()
    {
        InvokeRepeating("TrackPlayer", 0.0f, 0.02f);
        float lerpVal = 0.0f;
        Color c = Color.white;
        while (lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 2.5f;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            spotlight.pointLightOuterAngle = Mathf.Lerp(110, 25, lerpVal);
            spotlight.intensity = Mathf.Lerp(5, 10, lerpVal);
            c.r = Mathf.Lerp(Color.white.r, Color.red.r, lerpVal);
            c.g = Mathf.Lerp(Color.white.g, Color.red.g, lerpVal);
            c.b = Mathf.Lerp(Color.white.b, Color.red.b, lerpVal);
            spotlight.color = c;
            yield return null;
        }
        gameState = GameState.attacking;
    }

    void TrackPlayer()
    {
        rotator.right = player.position - spotlight.transform.position;
    }

    protected override IEnumerator CancelAttack()
    {
        Debug.Log("wot in tarnation?");
        CancelInvoke("TrackPlayer");
        StopCoroutine(Attack());
        float originRotation = rotator.rotation.z;

        float lerpVal = 0.0f;
        Color c = Color.red;
        while(lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 2.5f;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            spotlight.pointLightOuterAngle = Mathf.Lerp(25, 110, lerpVal);
            spotlight.intensity = Mathf.Lerp(10, 5, lerpVal);
            c.r = Mathf.Lerp(Color.red.r, Color.white.r, lerpVal);
            c.g = Mathf.Lerp(Color.red.g, Color.white.g, lerpVal);
            c.b = Mathf.Lerp(Color.red.b, Color.white.b, lerpVal);
            spotlight.color = c;

            if (direction.x < 0)
            {
                rotator.rotation = Quaternion.AngleAxis(Mathf.Lerp(originRotation, 180.0f, lerpVal), Vector3.forward);
            }
            else
            {
                rotator.rotation = Quaternion.AngleAxis(Mathf.Lerp(originRotation, 0.0f, lerpVal), Vector3.forward);
            }

            yield return null;
        }
        gameState = GameState.patrolling;
    }

    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(reactionTime);

        while (true)
        {
            GlueShot glue = Instantiate(pref_GlueShot, spotlight.transform.position + ((Vector3)direction * 0.5f), Quaternion.identity).GetComponent<GlueShot>();
            glue.Fire(spotlight.transform.position, player.position);
            yield return new WaitForSeconds(shotCooldown);
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
