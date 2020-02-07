using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [Header("Walking Enemy Info")]
    [SerializeField] float speed = 5;
    [SerializeField] float turnTime = 1.0f; //how long does he stay at each end of the range before turning around?
    [SerializeField] float reactionTime = 1.0f;
    [SerializeField] Transform rotator = null;
    [SerializeField] GameObject pref_GlueShot = null;

    [SerializeField] BoxCollider2D groundDetector = null;

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
        direction = new Vector2(-1.0f, 0.0f);
        timer = turnTime;
        xScale = transform.localScale.x;
        rotator.right = Vector3.left;
        Vector2 offset = groundDetector.offset;
        offset.x = -GetComponent<BoxCollider2D>().size.x / 2;
        groundDetector.offset = offset;
    }

    // Update is called once per frame
    new void Update()
    {
        //get a better method than adding a new vector every frame
        raycastHit = Physics2D.Raycast(transform.position + new Vector3(0.0f, 0.5f, 0.0f), direction, 3.0f, LayerMask.GetMask("environment"));
        scale = transform.localScale;
        transform.localScale = scale;

        base.Update();
    }


    protected override void Move()
    {
        if(isAtEnd || raycastHit.collider != null)
        {
            if (!turning)
            {
                StartCoroutine(SwapDirections());
            }
        }
        else
        {
            transform.Translate(direction * speed);
        }
    }

    IEnumerator SwapDirections()
    {
        turning = true;
        yield return new WaitForSeconds(turnTime);
        float lerpVal = direction.x < 0 ? 0.0f : 1.0f;
        Vector3 right = Vector3.zero;
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
        float lerpVal = 0.0f;
        Color c = Color.white;
        while (lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 2.5f;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            spotlight.pointLightOuterAngle = Mathf.Lerp(110, 25, lerpVal);
            c.r = Mathf.Lerp(Color.white.r, Color.red.r, lerpVal);
            c.g = Mathf.Lerp(Color.white.g, Color.red.g, lerpVal);
            c.b = Mathf.Lerp(Color.white.b, Color.red.b, lerpVal);
            spotlight.color = c;
            yield return null;
        }
        gameState = GameState.attacking;
    }

    protected override IEnumerator CancelAttack()
    {
        float lerpVal = 1.0f;
        Color c = Color.red;
        while(lerpVal > 0.0f)
        {
            lerpVal -= Time.deltaTime * 2.5f;
            if (lerpVal < 0.0f) lerpVal = 0.0f;

            spotlight.pointLightOuterAngle = Mathf.Lerp(110, 25, lerpVal);
            c.r = Mathf.Lerp(Color.white.r, Color.red.r, lerpVal);
            c.g = Mathf.Lerp(Color.white.g, Color.red.g, lerpVal);
            c.b = Mathf.Lerp(Color.white.b, Color.red.b, lerpVal);
            spotlight.color = c;
            yield return null;
        }
        gameState = GameState.patrolling;
    }

    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(reactionTime);
        //Instantiate(pref_GlueShot, transform.position, Quaternion.identity);
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
