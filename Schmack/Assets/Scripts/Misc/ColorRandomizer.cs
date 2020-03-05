using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    UnityEngine.Experimental.Rendering.LWRP.Light2D light;
    [SerializeField] float waitTime = 1.0f;
    [SerializeField] float rateOfChange = 1.0f;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Animator>())
        {
            animator = GetComponent<Animator>();
            animator.Play("clubLight", 0, Random.Range(0.0f, 1.0f));
        }

        light = gameObject.GetComponent<UnityEngine.Experimental.Rendering.LWRP.Light2D>();
        StartCoroutine(LerpRandomizedColor());
    }

    IEnumerator LerpRandomizedColor()
    {
        float lerpVal = 0.0f;
        Color origin;
        Color current = Color.white;
        Color target;
        while (true)
        {
            lerpVal = 0.0f;
            origin = light.color;
            target = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            yield return new WaitForSeconds(waitTime);

            while(lerpVal < 1.0f)
            {
                lerpVal += Time.deltaTime * rateOfChange;
                if (lerpVal > 1.0f) lerpVal = 1.0f;

                current.r = Mathf.Lerp(origin.r, target.r, lerpVal);
                current.g = Mathf.Lerp(origin.g, target.g, lerpVal);
                current.b = Mathf.Lerp(origin.b, target.b, lerpVal);

                light.color = current;

                yield return null;
            }
        }
    }
}
