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

    Animator animator;
    SoundManager sound;

    //hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate hate
    bool powershot = false; //is the current held shot a powershot? 

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

        animator = FindObjectOfType<Player>().gameObject.GetComponent<Animator>();
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
        }
    }




    protected void HandleInput()
    {
        Vector2 directionInput = controls.Player.Aim.ReadValue<Vector2>();
        powerInput = controls.Player.Draw.ReadValue<float>();

        if (powerInput < 1.0f)
            if (directionInput.sqrMagnitude > 0.0f) direction = directionInput;
            else StartCoroutine(DelayAimReset());
        else
        {
            if (directionInput.sqrMagnitude > 0.81f) direction = directionInput;
        }


    }

    IEnumerator DelayAimReset()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        direction = gameObject.transform.parent.localScale.x == -1 ? Vector2.left : Vector2.right;
    }

    public void FlipDirection()
    {
        direction.x *= -1;
    }

    void DrawBack()
    {
        if (state != State.drawn)
        {
            StartCoroutine(DisplayBigArrow());
            state = State.drawn;
            powershot = false;
        }

        animator.SetTrigger("draw");

        if (Rumble.rumble != 0.1f) Rumble.SetRumble(0.1f);
        sound.Play("BowDraw", 0.85f, 1.15f);
        sound.Play("Rumble");
    }

    void Fire()
    {
        state = State.fired;
        frameDelay = 0;
        StartCoroutine(Rumble.BurstRumble(1.0f, 0.1f));

        numArrows--;
        animator.SetTrigger("fire");

        Time.timeScale = timeScaleMax;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        GameObject newArrow = Instantiate(pref_arrow, GO_referencePoint.transform.position, GO_referencePoint.transform.rotation);
        if (powershot) newArrow.GetComponent<Arrow>().SetPowerShot(true);

        if (powershot) newArrow.transform.localScale *= 1.75f;

        if (inFlow) newArrow.GetComponent<Arrow>().AddForce(direction * flow_shotPower);
        else newArrow.GetComponent<Arrow>().AddForce(direction * noFlow_shotPower);

        arrows.Add(newArrow);

        if (inFlow) gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * flow_knockbackForce, true);
        else gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * noFlow_knockbackForce, true);

        sound.Stop("Rumble");
        sound.Play("BowFire", 0.85f, 1.15f);
    }

    public void EnableFire()
    {
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
            powershot = true;
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
