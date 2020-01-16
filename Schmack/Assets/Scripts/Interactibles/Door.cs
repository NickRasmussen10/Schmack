using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Controllable
{
    [SerializeField] float openingSize = 5.0f;
    [SerializeField] float openingSpeed = 1.0f;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override IEnumerator Activate()
    {
        isActivated = true;
        float lerpVal = 0.0f;
        Vector3 originalPosition = transform.position;
        while(lerpVal < 1.0f)
        {
            lerpVal += openingSpeed * Time.deltaTime;
            if (lerpVal > 1.0f) lerpVal = 1.0f;
            transform.position = new Vector3(originalPosition.x,
                                            Mathf.Lerp(originalPosition.y, originalPosition.y + openingSize, lerpVal),
                                            originalPosition.z);
            yield return null;
        }
        
    }

    protected override IEnumerator Deactivate()
    {
        isActivated = false;
        float lerpVal = 1.0f;
        Vector3 originalPosition = transform.position;
        while (lerpVal > 0.0f)
        {
            lerpVal -= openingSpeed * Time.deltaTime;
            if (lerpVal < 0.0f) lerpVal = 0.0f;
            transform.position = new Vector3(originalPosition.x,
                                            Mathf.Lerp(originalPosition.y - openingSize, originalPosition.y, lerpVal),
                                            originalPosition.z);
            yield return null;
        }
    }
}
