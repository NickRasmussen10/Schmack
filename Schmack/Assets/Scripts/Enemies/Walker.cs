using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : Enemy
{
    [Header("Walking Enemy Info")]
    [SerializeField] float speed = 5;
    [SerializeField] float waitTime = 1.0f; //how long does he stay at each end of the range before turning around?
    [SerializeField] Transform rotator;
    //[SerializeField] float visionRange = 1.0f;

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
        rotator.right = (rotator.position + new Vector3(-1, 0, 0)) - rotator.position;
    }

    // Update is called once per frame
    new void Update()
    {
        //get a better method than adding a new vector every frame
        raycastHit = Physics2D.Raycast(transform.position + new Vector3(0.0f, 0.5f, 0.0f), direction, 3.0f, LayerMask.GetMask("environment"));
        //Move();
        scale = transform.localScale;
        scale.x = isReversed ? xScale : -xScale;
        transform.localScale = scale;
        //audioMan.PlaySound("Enemy");

        base.Update();
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
        else if (SeesPlayer())
        {
            StartCoroutine(Attack());
        }
        else
        {
            transform.Translate(direction * speed);
        }
    }

    protected override IEnumerator Attack()
    {
        StartCoroutine(OhHello());
        while (true)
        {
            rotator.right = rotator.position - player.position;
            if (transform.localScale.x > 0) rotator.right *= -1;
            yield return null;
        }
    }

    IEnumerator OhHello()
    {
        float lerpVal = 0.0f;
        Color c = light.color;
        while(lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 2.5f;
            light.pointLightOuterAngle = Mathf.Lerp(110, 25, lerpVal);
            c.r = Mathf.Lerp(Color.white.r, Color.red.r, lerpVal);
            c.g = Mathf.Lerp(Color.white.g, Color.red.g, lerpVal);
            c.b = Mathf.Lerp(Color.white.b, Color.red.b, lerpVal);
            light.color = c;
            yield return null;
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
