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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector2(Input.GetAxis("RightHorizontal"), Input.GetAxis("RightVertical")).normalized;

        powerInput = Input.GetAxis("Fire1");

        if(powerInput == 1)
        {
            isDrawnBack = true;
        }
        else if(powerInput == 0 && isDrawnBack)
        {
            Debug.Log("Pew");
            isDrawnBack = false;
            GameObject newArrow = Instantiate(pref_arrow, transform.position, new Quaternion(direction.x, direction.y, 0.0f, 0.0f));
            newArrow.GetComponent<Arrow>().AddForce(direction * shotPower);
            arrows.Add(newArrow);
        }
    }
}
