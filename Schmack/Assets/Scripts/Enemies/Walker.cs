using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [Header("Walking Enemy Behavioral Variables")]
    [SerializeField] float turnTime = 1.0f; //how long does he stay at each end of the range before turning around?
    [SerializeField] float reactionTime = 1.0f; //how long in seconds does it take for the enemy to attack the player?
    [SerializeField] float shotCooldown = 1.0f; //how long in seconds does it take for the enemy to fire another shot?

    [Header("Firing")]
    [SerializeField] Transform rotator = null; //"head" that rotates to look at player / facing direction
    [SerializeField] GameObject pref_GlueShot = null;

    CollisionPacket collPacket_ground;
    CollisionPacket collPacket_wall;
    [Header("Child Colliders, nuffin' ta see here")]
    [SerializeField] GameObject groundCollider;
    [SerializeField] GameObject wallCollider;

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
        //Vector2 offset = groundCollider.offset;
        //offset.x = -(GetComponent<BoxCollider2D>().size.x / 2) - groundCollider.size.x / 2;
        //groundCollider.offset = offset;
        //offset = wallCollider.offset;
        //offset.x = -(GetComponent<BoxCollider2D>().size.x / 2) - wallCollider.size.x / 2;
        //wallCollider.offset = offset;

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
        //if the end of the platform has been detected (edge or wall)
        if (!collPacket_ground.isColliding || collPacket_wall.isColliding)
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
            transform.Translate(direction * speed * Time.deltaTime);
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
        if (lerpVal == 0.0f)
        {
            float rotation = 0.0f;
            while (lerpVal < 1.0f)
            {
                lerpVal += Time.deltaTime * 3.0f;
                if (lerpVal > 1.0f) lerpVal = 1.0f;

                //lerp head rotation
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

                //lerp head rotation
                rotation = Mathf.Lerp(180.0f, 0.0f, lerpVal);
                rotator.rotation = Quaternion.AngleAxis(rotation, Vector3.forward);
                yield return null;
            }
        }

        //mark coroutine as complete
        turning = false;

        //change direction
        direction.x *= -1;

        //update detection collider positions so that they are at front enemy in new direction
        Vector3 scale = groundCollider.transform.localScale;
        scale.x *= -1;
        groundCollider.transform.localScale = scale;
        scale = wallCollider.transform.localScale;
        scale.x *= -1;
        wallCollider.transform.localScale = scale;
    }

    /// <summary>
    /// Handles "calculation delay time" and bringing the enemy to an attack state
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator PrepAttack()
    {
        //track the player
        InvokeRepeating("TrackPlayer", 0.0f, 0.02f);

        float lerpVal = 0.0f;
        Color c = Color.white;

        //go to attack light
        while (lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 2.5f;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            //lerp light intensity and angle
            spotlight.pointLightOuterAngle = Mathf.Lerp(110, 25, lerpVal);
            spotlight.intensity = Mathf.Lerp(5, 10, lerpVal);

            //lerp light color
            c.r = Mathf.Lerp(Color.white.r, Color.red.r, lerpVal);
            c.g = Mathf.Lerp(Color.white.g, Color.red.g, lerpVal);
            c.b = Mathf.Lerp(Color.white.b, Color.red.b, lerpVal);
            spotlight.color = c;
            yield return null;
        }

        gameState = GameState.attacking;
        attack = null;
    }


    /// <summary>
    /// Rotates light towards player
    /// </summary>
    void TrackPlayer()
    {
        rotator.right = player.position - spotlight.transform.position;
        //rotator.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2((player.position - spotlight.gameObject.transform.position).y, (player.position - spotlight.gameObject.transform.position).x) * Mathf.Rad2Deg);
    }


    /// <summary>
    /// Brings enemy back to patrolling state from attack
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator CancelAttack()
    {
        //stop tracking and player player
        CancelInvoke("TrackPlayer");
        StopCoroutine(attack);

        float lerpVal = 0.0f;
        Color c = Color.red;

        //lerp back to patrolling state
        while (lerpVal < 1.0f)
        {
            Debug.Log(lerpVal);
            lerpVal += Time.deltaTime * 2.5f;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            //lerp light angle and intensity
            spotlight.pointLightOuterAngle = Mathf.Lerp(25, 110, lerpVal);
            spotlight.intensity = Mathf.Lerp(10, 5, lerpVal);

            //lerp light color
            c.r = Mathf.Lerp(Color.red.r, Color.white.r, lerpVal);
            c.g = Mathf.Lerp(Color.red.g, Color.white.g, lerpVal);
            c.b = Mathf.Lerp(Color.red.b, Color.white.b, lerpVal);
            spotlight.color = c;

            yield return null;
        }

        float originRotation = rotator.rotation.z;

        //lerp head rotation to correct position based on enemy's direction
        if (direction.x < 0)
        {
            rotator.rotation = Quaternion.AngleAxis(Mathf.Lerp(originRotation, 180.0f, lerpVal), Vector3.forward);
        }
        else
        {
            rotator.rotation = Quaternion.AngleAxis(Mathf.Lerp(originRotation, 0.0f, lerpVal), Vector3.forward);
        }

        gameState = GameState.patrolling;
    }


    /// <summary>
    /// Handles enemy attacking player
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Attack()
    {
        //wait for reactionTime seconds
        yield return new WaitForSeconds(reactionTime);

        //keep looping until coroutine is stopped elsewhere     NOTE: this could potentially be handled as a boolean flag outside the method?
        while (true)
        {
            //create and fire a glue shot
            //GlueShot glue = Instantiate(pref_GlueShot, spotlight.transform.position + ((Vector3)direction * 0.5f), Quaternion.identity).GetComponent<GlueShot>();
            //glue.Fire(spotlight.transform.position, player.position);

            //wait for shotCooldown seconds and repeat
            yield return new WaitForSeconds(shotCooldown);
        }
    }


    /// <summary>
    /// recieves a collision packet from the ground reporter
    /// </summary>
    /// <param name="packet"></param>
    void GetCollisionReportGround(CollisionPacket packet)
    {
        collPacket_ground = packet;
    }


    /// <summary>
    /// recieves a collision packet from the wall reporter
    /// </summary>
    /// <param name="packet"></param>
    void GetCollisionReportWall(CollisionPacket packet)
    {
        collPacket_wall = packet;
    }
}
