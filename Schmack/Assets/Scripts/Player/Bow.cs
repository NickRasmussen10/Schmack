using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public Vector2 direction;
    public bool isDrawnBack = false;
    public bool fire = false;
    float powerInput = 0.0f;

    List<GameObject> arrows = new List<GameObject>();

    [SerializeField] GameObject pref_indicator;
    GameObject indicator;
    float indicatorDistance = 5.0f;

    [SerializeField] GameObject[] arms;

    [Header("Firing")]
    [SerializeField] GameObject pref_arrow;
    [SerializeField] float shotPower = 1.0f;
    [SerializeField] float shotCooldown = 1.0f;
    [SerializeField] float knockbackForce = 1.0f;

    [SerializeField] GameObject shootPoint;
    float coolDownTimer = 0.0f;
    int frameDelay = 0;

    [Header("Timescaling")]
    [SerializeField] float timeScaleMin = 0.5f; //the slowest the game will go on bow drawback
    [SerializeField] float timeScaleMax = 1.0f; //the fastest the game will go otherwise (1.0f is normal)
    [SerializeField] float timeScaleTime = 3.0f;
    public float timeScale;

    float lerpVal = 0.0f;
    float dilationTime = 0.0f;
    AudioManager audioMan;
    // Start is called before the first frame update
    void Start()
    {
        audioMan = AudioManager.instance;
        if (audioMan == null)
        {
            Debug.LogError("No audiomanager found");
        }
        indicator = Instantiate(pref_indicator);
        timeScale = timeScaleMax;
        lerpVal = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDrawnBack)
        {
            audioMan.PlaySound("BowDraw");
            Debug.Log("bowdraw play");
        }
        if (fire)
        {
            fire = false;
            audioMan.PlaySound("BowFire");
            Debug.Log("bowfire play");
            //newaudio.PlaySound("ArrowFly");
        }

        direction = new Vector2(Input.GetAxis("RightHorizontal"), Input.GetAxis("RightVertical")).normalized;
        powerInput = Input.GetAxis("Fire1");

        if(direction.sqrMagnitude == 0)
            direction.x = gameObject.GetComponent<PlayerMovement>().direction.x;
        SetIndicatorPosition();

        if(coolDownTimer > 0) coolDownTimer -= Time.deltaTime;

        if(powerInput == 1 && coolDownTimer <= 0.0f)
        {
            if (!isDrawnBack) isDrawnBack = true;
            if (frameDelay == 10)
            {
                TimeDilation(true);
            }
            else
            {
                frameDelay++;
            }
        }
        else if(powerInput == 0 && isDrawnBack)
        {
            fire = true;
            frameDelay = 0;
            Time.timeScale = timeScaleMax;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            coolDownTimer = shotCooldown;
            isDrawnBack = false;
            GameObject newArrow = Instantiate(pref_arrow, shootPoint.transform.position, new Quaternion(direction.x, direction.y, 0.0f, 0.0f));
            newArrow.GetComponent<Arrow>().AddForce(direction * shotPower);
            arrows.Add(newArrow);
            gameObject.GetComponent<PlayerMovement>().AddKnockback(-direction * knockbackForce, true);
        }
        else
        {
            isDrawnBack = false;
        }
    }

    void TimeDilation(bool slowingDown)
    {
        if (slowingDown)
        {
            if (lerpVal > 0)
                lerpVal -= Time.deltaTime * 3f;
            if (lerpVal < 0)
                lerpVal = 0;
            Time.timeScale = Mathf.Lerp(timeScaleMin, timeScaleMax, lerpVal);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        else
        {
            if(dilationTime <= 0)
            {
                if (lerpVal < 1.0f)
                    lerpVal += Time.deltaTime;
                if (lerpVal > 1.0f)
                    lerpVal = 1.0f;
                lerpVal += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(timeScaleMin, timeScaleMax, lerpVal);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
    }

    /// <summary>
    /// note: this implementation of indicator will become obsolete when player arm/bow rotation is implemented
    /// </summary>
    void SetIndicatorPosition()
    {
        indicator.transform.position = (Vector2)transform.position + (direction * indicatorDistance);
    }
}
