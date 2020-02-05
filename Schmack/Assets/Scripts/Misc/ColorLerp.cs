using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLerp : MonoBehaviour
{
    [SerializeField] List<Color> colors = new List<Color>();
    // Start is called before the first frame update
    void Start()
    {
        if(colors.Count > 1)
        {
            StartCoroutine(LerpColors());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator LerpColors()
    {
        int current = 0;
        int next = 1;
        float lerpVal = 0.0f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        while (true)
        {
            while(lerpVal < 1.0f)
            {
                lerpVal += Time.deltaTime * 0.01f;
                color.r = Mathf.Lerp(colors[current].r, colors[next].r, lerpVal);
                color.g = Mathf.Lerp(colors[current].g, colors[next].g, lerpVal);
                color.b = Mathf.Lerp(colors[current].b, colors[next].b, lerpVal);
                color.a = Mathf.Lerp(colors[current].a, colors[next].a, lerpVal);
                sr.color = color;
                yield return null;
            }
            lerpVal = 0.0f;
            current += 1;
            if (current == colors.Count) current = 0;

            next = current + 1;
            if (next == colors.Count) next = 0;
        }
    }
}
