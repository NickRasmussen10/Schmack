using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    public Vector2 direction;
    public bool isDrawnBack = false;
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

    AudioManager audioMan;

    // Start is called before the first frame update
    protected void Start()
    {
        Activate();
        audioMan = AudioManager.instance;
        if (audioMan == null)
        {
            Debug.LogError("No audiomanager found");
        }
        timeScale = timeScaleMax;


        numArrows = maxArrows;
    }

    // Update is called once per frame
    protected void Update()
    {
        Debug.Log("base bow update");
        HandleInput();
        HandleFiring();
        PlaySounds();
    }


    protected void PlaySounds()
    {
        if (isDrawnBack)
        {
            audioMan.PlaySound("BowDraw");
        }
        if (fire)
        {
            fire = false;
            audioMan.PlaySound("BowFire");
            //newaudio.PlaySound("ArrowFly");
        }
    }

    protected void HandleInput()
    {
        direction.x = Input.GetAxis("RightHorizontal");
        direction.y = Input.GetAxis("RightVertical");
        powerInput = Input.GetAxis("Fire1");

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
        if (!isDrawnBack) isDrawnBack = true;
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
        frameDelay = 0;
        StopCoroutine("TimeDilationDown");

        Time.timeScale = timeScaleMax;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        isDrawnBack = false;
        GameObject newArrow = Instantiate(pref_arrow, transform.position, new Quaternion(direction.x, direction.y, 0.0f, 0.0f));

        if(inFlow) newArrow.GetComponent<Arrow>().AddForce(direction * flow_shotPower);
        else newArrow.GetComponent<Arrow>().AddForce(direction * noFlow_shotPower);

        arrows.Add(newArrow);

        if(inFlow) gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * flow_knockbackForce, true);
        else gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * noFlow_knockbackForce, true);
    }

    IEnumerator TimeDilationDown()
    {
        float lerpVal = 1.0f;
        while (lerpVal > 0)
        {
            lerpVal -= Time.deltaTime * 3f;
            if (lerpVal < 0) lerpVal = 0;
            Time.timeScale = Mathf.Lerp(timeScaleMin, timeScaleMax, lerpVal);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
    }

    //IEnumerator BowRecharge()
    //{
    //    rechargeRunning = true;
    //    while (true)
    //    {
    //        if (numArrows < maxArrows)
    //        {
    //            numArrows++;
    //        }
    //        yield return new WaitForSeconds(rechargeTime);
    //    }
    //}

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
    }

    public void Deactivate()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}
