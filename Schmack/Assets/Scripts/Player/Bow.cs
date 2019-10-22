using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    Vector2 direction;
    bool isDrawnBack = false;
    float powerInput = 0.0f;

    List<GameObject> arrows = new List<GameObject>();
    [SerializeField] GameObject pref_arrow;
    [SerializeField] float shotPower = 1.0f;
    [SerializeField] float shotCooldown = 1.0f;
    [SerializeField] float knockbackForce = 1.0f;

    [SerializeField] GameObject shootPoint;
    float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector2(Input.GetAxis("RightHorizontal"), Input.GetAxis("RightVertical")).normalized;
        powerInput = Input.GetAxis("Fire1");

        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if(powerInput == 1 && timer <= 0.0f)
        {
            isDrawnBack = true;
        }
        else if(powerInput == 0 && isDrawnBack)
        {
            timer = shotCooldown;
            isDrawnBack = false;
            GameObject newArrow = Instantiate(pref_arrow, shootPoint.transform.position, new Quaternion(direction.x, direction.y, 0.0f, 0.0f));
            newArrow.GetComponent<Arrow>().AddForce(direction * shotPower);
            arrows.Add(newArrow);
            gameObject.GetComponent<PlayerMovement>().AddKnockback(-direction * knockbackForce, true);
        }
    }
}
