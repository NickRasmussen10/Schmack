using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Controllable
{
    [SerializeField] float openingSize = 5.0f;
    [SerializeField] float openingSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SwapWeapon") && !isActivated)
        {
            StartCoroutine("Activate");
        }
    }

    protected override IEnumerator Activate()
    {
        isActivated = true;
        float lerpVal = 0.0f;
        Vector3 originalPosition = transform.position;
        while(lerpVal < 1.0f)
        {
            transform.position = new Vector3(originalPosition.x,
                                            Mathf.Lerp(originalPosition.y, originalPosition.y + openingSize, lerpVal),
                                            originalPosition.z);
            lerpVal += openingSpeed * Time.deltaTime;
            if (lerpVal > 1.0f) lerpVal = 1.0f;
            yield return null;
        }
        
    }
}
