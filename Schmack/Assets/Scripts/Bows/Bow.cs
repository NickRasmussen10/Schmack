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
        powerShot,
        fired
    }

    public State state;

    public int numArrows;
    List<GameObject> arrows = new List<GameObject>();

    [SerializeField] GameObject pref_arrow = null;
    [SerializeField] int maxArrows = 3;
    [SerializeField] float rechargeTime = 1.0f;
    [SerializeField] float timeToPowershot = 1.0f; // this field is implemented independant of game's timescale feature


    [Header("Firing - Flow")]
    [SerializeField] float flow_shotPower = 1500.0f;
    [SerializeField] float flow_knockbackForce = 700.0f;

    [Header("Firing - Slow Flow")]
    [SerializeField] float noFlow_shotPower = 1500.0f;
    [SerializeField] float noFlow_knockbackForce = 700.0f;

    [Header("Gross Yucky References")]
    [SerializeField] Transform referencePoint = null;
    [SerializeField] SpriteRenderer[] powerShotEffects;

    Coroutine recharge;
    Coroutine powershotTimer;

    Animator animator;
    SoundManager sound;


    // Start is called before the first frame update
    protected void Start()
    {
        Activate();
        numArrows = maxArrows;

        animator = FindObjectOfType<Player>().gameObject.GetComponent<Animator>();
        sound = FindObjectOfType<SoundManager>();

        SetPowershotEffects(false);
    }

    // Update is called once per frame
    protected void Update()
    {
        HandleInput();

        switch (state)
        {
            case State.idle:
                if (powerInput > 0.9f && numArrows > 0) DrawBack();
                if (powerShotEffects[0].enabled) SetPowershotEffects(false);
                break;
            case State.drawn:
                if (powerInput < 0.9f) Fire(false);
                break;
            case State.powerShot:
                if (powerInput < 0.9f) Fire(true);
                break;
            case State.fired:
                animator.SetBool("drawn", false);
                animator.SetBool("fired", false);
                state = State.idle;
                break;
            default:
                break;
        }


        ///Dear future me:
        ///I hope this comment finds you in good health. I'd like to sincerely apologise for the code here.
        ///It was late and I was desperate to fix this timeScale issue and I turned to dishonorable means. 
        ///Is it effective? Yes. Is it scalable? No. Will it cuase issues later? Well you're reading this so
        ///yikes bud, sorry about that. 
        ///                 Get fucked. -Nick
        //if (powerInput <= 0.5f && Time.timeScale != 1.0f)
        //{
        //    Time.timeScale = 1.0f;
        //    Time.fixedDeltaTime = 0.02f * Time.timeScale;
        //}
    }




    protected void HandleInput()
    {
        Vector2 directionInput = Inputs.controls.Player.Aim.ReadValue<Vector2>();
        powerInput = Inputs.controls.Player.Draw.ReadValue<float>();

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
        direction = gameObject.transform.parent.localScale.x < 0 ? Vector2.left : Vector2.right;
    }

    public void FlipDirection()
    {
        direction.x *= -1;
    }

    void DrawBack()
    {

        state = State.drawn;
        powershotTimer = StartCoroutine(ChargePowershot());
        animator.SetBool("drawn", true);

        if (Rumble.rumble != 0.1f) Rumble.SetRumble(0.1f);
        sound.Play("BowDraw", 0.85f, 1.15f);
        sound.Play("Rumble");
    }

    void Fire(bool isPowershot)
    {
        state = State.fired;
        StartCoroutine(Rumble.BurstRumble(1.0f, 0.1f));
        StopCoroutine(powershotTimer);

        numArrows--;
        animator.SetBool("fired", true);

        SetPowershotEffects(false);

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Debug.Log(Time.timeScale);

        GameObject newArrow = Instantiate(pref_arrow, referencePoint.position, referencePoint.rotation);
        if (isPowershot)
        {
            newArrow.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.5f, 0.5f);
            newArrow.GetComponent<Arrow>().SetPowerShot(true);
        }
        newArrow.GetComponent<Arrow>().AddForce(direction * (inFlow ? flow_shotPower : noFlow_shotPower));

        arrows.Add(newArrow);

        gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * (inFlow ? flow_knockbackForce : noFlow_knockbackForce), true);

        sound.Stop("Rumble");
        sound.Play("BowFire", 0.85f, 1.15f);
    }

    public void FireArrow()
    {
        GameObject newArrow = Instantiate(pref_arrow, referencePoint.position, referencePoint.rotation);
        if (true)
        {
            newArrow.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.5f, 0.5f);
            newArrow.GetComponent<Arrow>().SetPowerShot(true);
        }
        newArrow.GetComponent<Arrow>().AddForce(direction * (inFlow ? flow_shotPower : noFlow_shotPower));

        arrows.Add(newArrow);

    }

    IEnumerator ChargePowershot()
    {
        float timer = 0.0f;
        while (timer < timeToPowershot)
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= timeToPowershot && state == State.drawn)
            {
                state = State.powerShot;
                SetPowershotEffects(true);
            }
            yield return null;
        }
    }

    void SetPowershotEffects(bool isOn)
    {
        foreach (SpriteRenderer spriteRenderer in powerShotEffects)
        {
            spriteRenderer.enabled = isOn;
        }
    }

    public void EnableFire()
    {
        state = State.idle;
        animator.SetBool("drawn", false);
        animator.SetBool("fired", false);
    }

    IEnumerator BowRecharge()
    {
        while (true)
        {
            if (numArrows < maxArrows)
            {
                yield return new WaitForSeconds(rechargeTime);
                numArrows++;
            }
            yield return null;
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        state = State.idle;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    void GetCollisionReportGround(CollisionPacket packet)
    {
        if (packet.isColliding && recharge == null)
        {
            recharge = StartCoroutine(BowRecharge());
        }
        else if (!packet.isColliding && recharge != null)
        {
            StopCoroutine(recharge);
            recharge = null;
        }

        if (numArrows < maxArrows)
        {
            numArrows++;
        }
    }

    void GetCollisionReportBackLegs(CollisionPacket packet)
    {
        if (packet.isColliding && recharge == null)
        {
            recharge = StartCoroutine(BowRecharge());
        }
        else if (!packet.isColliding && recharge != null)
        {
            StopCoroutine(recharge);
            recharge = null;
        }
    }
}
