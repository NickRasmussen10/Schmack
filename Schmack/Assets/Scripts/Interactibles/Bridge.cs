using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : Controllable
{
    [SerializeField] float openingSpeed = 5.0f;
    [SerializeField] float openingSize = 0.9f;

    Vector3 minPosition;
    Vector3 maxPosition;
    float lerpVal = 0.0f;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        openingSize = gameObject.GetComponent<BoxCollider2D>().bounds.size.x * 0.8f;
        minPosition = transform.position;
        maxPosition = new Vector3(minPosition.x + openingSize, minPosition.y, minPosition.z);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    protected override IEnumerator Activate()
    {
        isActivated = true;
        while(lerpVal < 1.0f)
        {
            lerpVal += openingSpeed * Time.deltaTime;
            if (lerpVal > 1.0f) lerpVal = 1.0f;
            transform.position = new Vector3(Mathf.Lerp(minPosition.x, maxPosition.x, lerpVal),
                                            minPosition.y,
                                            minPosition.z);
            yield return null;
        }
    }

    protected override IEnumerator Deactivate()
    {
        isActivated = false;
        while (lerpVal > 0.0f)
        {
            lerpVal -= openingSpeed * Time.deltaTime;
            if (lerpVal < 0.0f) lerpVal = 0.0f;
            transform.position = new Vector3(Mathf.Lerp(minPosition.x, maxPosition.x, lerpVal),
                                            minPosition.y,
                                            minPosition.z);
            yield return null;
        }
    }
}
