using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    SpriteRenderer bigArrowSprite;
    bool fireOnRightTrigger = true;

    public Vector2 direction;
    public bool inFlow;
    protected float powerInput = 0.0f;

    public enum State
    {
        idle,
        drawn,
        fired
    }

    public State state;

    public int numArrows;
    List<GameObject> arrows = new List<GameObject>();

    [SerializeField] GameObject pref_arrow = null;
    [SerializeField] int maxArrows = 3;
    [SerializeField] float rechargeTime = 1.0f;


    [Header("Firing - Flow")]
    [SerializeField] float flow_shotPower = 1500.0f;
    [SerializeField] float flow_knockbackForce = 700.0f;

    [Header("Firing - Slow Flow")]
    [SerializeField] float noFlow_shotPower = 1500.0f;
    [SerializeField] float noFlow_knockbackForce = 700.0f;

    [Header("Timescaling")]
    [SerializeField] float timeScaleMin = 0.1f; //the slowest the game will go on bow drawback
    [SerializeField] float timeScaleMax = 1.0f; //the fastest the game will go otherwise (1.0f is normal)
    public float timeScale;

    [Header("Gross Yucky References")]
    [SerializeField] GameObject GO_referencePoint = null;
    [SerializeField] GameObject bigArrow = null;

    Controls controls = null;

    Animator anim;
    SoundManager sound;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    protected void Start()
    {
        controls = transform.parent.gameObject.GetComponent<PlayerMovement>().controls;

        Activate();
        timeScale = timeScaleMax;


        numArrows = maxArrows;

        anim = GameObject.Find("arms").GetComponent<Animator>();
        sound = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    protected void Update()
    {
        HandleInput();

        switch (state)
        {
            case State.idle:
                if (powerInput > 0.9f && numArrows > 0) DrawBack();
                break;
            case State.drawn:
                TimeDilationDown();
                if (powerInput == 0) Fire();
                break;
            case State.fired:
                state = State.idle;
                break;
            default:
                break;
        }

        if (state != State.drawn)
        {
            if(bigArrowSprite.size.x > 0) bigArrowSprite.size = new Vector2(0, 0.75f);
            //if(Time.timeScale != timeScaleMax) Time.timeScale = timeScaleMax;
        }
    }




    protected void HandleInput()
    {
        direction = controls.Player.Aim.ReadValue<Vector2>();
        if (direction.sqrMagnitude > 0.1f) anim.SetBool("aim", true);
        else if (anim.GetBool("aim")) anim.SetBool("aim", false);

        powerInput = controls.Player.Draw.ReadValue<float>();

        if (direction.sqrMagnitude == 0)
            direction = gameObject.transform.parent.localScale.x == -1 ? Vector2.left : Vector2.right;
    }

    //protected void HandleFiring()
    //{

    //    if (numArrows > 0)
    //    {
    //        if (powerInput == 1 && !isDrawnBack)
    //        {
    //            DrawBack();
    //        }
    //        else if (powerInput == 0 && isDrawnBack)
    //        {
    //            Fire();
    //        }
    //    }
    //    else
    //    {
    //        isDrawnBack = false;
    //    }
    //}

    void DrawBack()
    {
        if (state != State.drawn)
        {
            StartCoroutine(DisplayBigArrow());
            state = State.drawn;
        }

        if (!anim.GetBool("isDrawn"))
        {
            anim.SetTrigger("draw");
            anim.SetBool("isDrawn", true);
            anim.SetBool("isFired", false);
        }

        if (Rumble.rumble != 0.1f) Rumble.SetRumble(0.1f);
        sound.Play("BowDraw");
        sound.Play("Rumble");

        //StartCoroutine(TimeDilationDown());

    }

    void Fire()
    {
        state = State.fired;
        frameDelay = 0;
        StartCoroutine(Rumble.BurstRumble(1.0f, 0.1f));

        numArrows--;
        anim.SetBool("isFired", true);
        anim.SetBool("isDrawn", false);

        Time.timeScale = timeScaleMax;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        GameObject newArrow = Instantiate(pref_arrow, GO_referencePoint.transform.position, GO_referencePoint.transform.rotation);

        if (inFlow) newArrow.GetComponent<Arrow>().AddForce(direction * flow_shotPower);
        else newArrow.GetComponent<Arrow>().AddForce(direction * noFlow_shotPower);

        arrows.Add(newArrow);

        if (inFlow) gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * flow_knockbackForce, true);
        else gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * noFlow_knockbackForce, true);

        sound.Stop("Rumble");
        sound.Play("BowFire");

        //StartCoroutine(Temp_FireDelay());
    }

    IEnumerator Temp_FireDelay()
    {
        yield return new WaitForSeconds(0.35f);
        state = State.idle;
    }

    public void LoadNextArrow()
    {
        state = State.idle;
    }

    IEnumerator DisplayBigArrow()
    {
        SpriteRenderer sr = bigArrow.GetComponent<SpriteRenderer>();
        Vector2 size = sr.size;
        while (size.x < 2.5)
        {
            size.x += 0.15f;
            if (size.x > 2.5f) size.x = 2.5f;
            sr.size = size;
            yield return new WaitForEndOfFrame();
        }
    }



    int frameDelay = 0; // I hate this, I wish it was a coroutine, but the coroutine was causing some pretty serious bugs
    void TimeDilationDown()
    {
        frameDelay++;
        if(frameDelay == 10)
        {
            Time.timeScale = timeScaleMin;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }

    void BowRecharge()
    {
        if (numArrows < maxArrows)
        {
            numArrows++;
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        InvokeRepeating("BowRecharge", rechargeTime, rechargeTime);
        bigArrowSprite = bigArrow.GetComponent<SpriteRenderer>();
        state = State.idle;
    }

    public void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
