using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    [SerializeField] GameObject bigArrow = null;
    SpriteRenderer bigArrowSprite;
    bool fireOnRightTrigger = true;

    public Vector2 direction;
    public bool isDrawnBack = false;
    public bool drawHeld = false;
    public bool fire = false;
    public bool inFlow;
    protected float powerInput = 0.0f;

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

    float coolDownTimer = 0.0f;
    int frameDelay = 0;

    [Header("Timescaling")]
    [SerializeField] float timeScaleMin = 0.1f; //the slowest the game will go on bow drawback
    [SerializeField] float timeScaleMax = 1.0f; //the fastest the game will go otherwise (1.0f is normal)
    public float timeScale;

    [Header("Gross Yucky References")]
    [SerializeField] GameObject GO_referencePoint = null;
    [SerializeField] Controls controls = null;

    Animator anim;

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
    }

    // Update is called once per frame
    protected void Update()
    {
        HandleInput();
        HandleFiring();

        if(!isDrawnBack && bigArrowSprite.size.x > 0)
        {
            bigArrowSprite.size = new Vector2(0, 0.75f);
        }
    }


   

    protected void HandleInput()
    {
        direction = controls.Player.Aim.ReadValue<Vector2>();
        if (direction.sqrMagnitude > 0.1f) anim.SetBool("aim", true);
        else if (anim.GetBool("aim")) anim.SetBool("aim", false);

        powerInput = controls.Player.Draw.ReadValue<float>();

        if (direction.sqrMagnitude == 0)
            direction.x = gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>().direction.x;
    }

    protected void HandleFiring()
    {
        if (coolDownTimer > 0) coolDownTimer -= Time.deltaTime;

        if (numArrows > 0)
        {
            if (powerInput == 1)
            {
                DrawBack();
            }
            else if (powerInput == 0 && isDrawnBack)
            {
                Fire();
            }
        }
        else
        {
            isDrawnBack = false;
        }
    }

    void DrawBack()
    {
        if (!isDrawnBack)
        {
            StartCoroutine(DisplayBigArrow());
            isDrawnBack = true;
        }

        if (!anim.GetBool("isDrawn"))
        {
            anim.SetTrigger("draw");
            anim.SetBool("isDrawn", true);
            anim.SetBool("isFired", false);
        }

        

        if (frameDelay == 10)
        {
            StartCoroutine("TimeDilationDown");
            //TimeDilation(true);
        }
        else
        {
            frameDelay++;
        }
    }

    void Fire()
    {
        numArrows--;
        fire = true;
        anim.SetBool("isFired", true);
        anim.SetBool("isDrawn", false);
        frameDelay = 0;
        StopCoroutine("TimeDilationDown");

        Time.timeScale = timeScaleMax;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        isDrawnBack = false;

        GameObject newArrow = Instantiate(pref_arrow, GO_referencePoint.transform.position, GO_referencePoint.transform.rotation);

        if (inFlow) newArrow.GetComponent<Arrow>().AddForce(direction * flow_shotPower);
        else newArrow.GetComponent<Arrow>().AddForce(direction * noFlow_shotPower);

        arrows.Add(newArrow);

        if (inFlow) gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * flow_knockbackForce, true);
        else gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * noFlow_knockbackForce, true);
    }

    IEnumerator DisplayBigArrow()
    {
        SpriteRenderer sr = bigArrow.GetComponent<SpriteRenderer>();
        Vector2 size = sr.size;
        while(size.x < 2.5)
        {
            size.x += 0.15f;
            if (size.x > 2.5f) size.x = 2.5f;
            sr.size = size;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator TimeDilationDown()
    {
        float lerpVal = 1.0f;

        while (lerpVal > 0)
        {
            lerpVal -= Time.deltaTime * 20f;
            if (lerpVal < 0) lerpVal = 0;
            Time.timeScale = Mathf.Lerp(timeScaleMin, timeScaleMax, lerpVal);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
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
    }

    public void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
