using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    Vector2 direction;
    bool isDrawnBack = false;
    float powerInput = 0.0f;

    List<GameObject> arrows = new List<GameObject>();

    [SerializeField] GameObject indicator;
    float indicatorDistance = 2.0f;

    [Header("Firing")]
    [SerializeField] GameObject pref_arrow;
    [SerializeField] float shotPower = 1.0f;
    [SerializeField] float shotCooldown = 1.0f;
    [SerializeField] float knockbackForce = 1.0f;

    [SerializeField] GameObject shootPoint;
    float coolDownTimer = 0.0f;

    [Header("Timescaling")]
    [SerializeField] float timeScaleMin = 0.5f; //the slowest the game will go on bow drawback
    [SerializeField] float timeScaleMax = 1.0f; //the fastest the game will go otherwise (1.0f is normal)

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector2(Input.GetAxis("RightHorizontal"), Input.GetAxis("RightVertical")).normalized;
        powerInput = Input.GetAxis("Fire1");

        if (direction.sqrMagnitude > 0) SetIndicatorPosition();

        if(coolDownTimer > 0) coolDownTimer -= Time.deltaTime;

        if(powerInput == 1 && coolDownTimer <= 0.0f)
        {
            Time.timeScale = timeScaleMin;
            isDrawnBack = true;
        }
        else if(powerInput == 0 && isDrawnBack)
        {
            Time.timeScale = timeScaleMax;
            coolDownTimer = shotCooldown;
            isDrawnBack = false;
            GameObject newArrow = Instantiate(pref_arrow, shootPoint.transform.position, new Quaternion(direction.x, direction.y, 0.0f, 0.0f));
            newArrow.GetComponent<Arrow>().AddForce(direction * shotPower);
            arrows.Add(newArrow);
            gameObject.GetComponent<PlayerMovement>().AddKnockback(-direction * knockbackForce, true);
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
