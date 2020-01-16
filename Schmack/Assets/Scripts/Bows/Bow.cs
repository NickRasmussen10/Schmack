using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public Vector2 direction;
    public bool isDrawnBack = false;
    public bool fire = false;
    protected float powerInput = 0.0f;

    List<GameObject> arrows = new List<GameObject>();

    [SerializeField] GameObject pref_indicator = null;
    [SerializeField] float indicatorDistance = 5.0f;
    GameObject indicator;



    [Header("Firing")]
    [SerializeField] GameObject pref_arrow = null;
    [SerializeField] float shotPower = 1500.0f;
    [SerializeField] float shotCooldown = 1.0f;
    [SerializeField] float knockbackForce = 700.0f;

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
    }

    // Update is called once per frame
    protected void Update()
    {
        HandleInput();
        SetIndicatorPosition();
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

        if (powerInput == 1 && coolDownTimer <= 0.0f)
        {
            DrawBack();
        }
        else if (powerInput == 0 && isDrawnBack)
        {
            Fire();
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
        fire = true;
        frameDelay = 0;
        StopCoroutine("TimeDilationDown");
        Time.timeScale = timeScaleMax;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        coolDownTimer = shotCooldown;
        isDrawnBack = false;
        GameObject newArrow = Instantiate(pref_arrow, transform.position, new Quaternion(direction.x, direction.y, 0.0f, 0.0f));
        newArrow.GetComponent<Arrow>().AddForce(direction * shotPower);
        arrows.Add(newArrow);
        gameObject.transform.parent.GetComponent<PlayerMovement>().AddKnockback(-direction * knockbackForce, true);
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

    /// <summary>
    /// note: this implementation of indicator will become obsolete when player arm/bow rotation is implemented
    /// </summary>
    protected void SetIndicatorPosition()
    {
        indicator.transform.position = (Vector2)transform.position + (direction * indicatorDistance);
    }

    public void Activate()
    {
        indicator = Instantiate(pref_indicator);
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        Destroy(indicator);
        gameObject.SetActive(false);
    }
}
