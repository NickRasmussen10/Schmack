using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.PostProcessing;
using UnityEngine.Rendering.PostProcessing;

public class PPPSwapper : MonoBehaviour
{
    [SerializeField] PostProcessVolume flow;
    [SerializeField] PostProcessVolume noFlow;
    [SerializeField] UnityEngine.Experimental.Rendering.LWRP.Light2D gloabalLight;

    PlayerMovement playerMovement;
    bool inFlow;
    
    //// Start is called before the first frame update
    void Start()
    {
        noFlow.weight = 1.0f;
        flow.weight = 0.0f;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    //// Update is called once per frame
    void Update()
    {
        if (playerMovement.inFlow != inFlow)
        {
            StartCoroutine(Flerp(playerMovement.inFlow));
        }
    }

    private void OnApplicationQuit()
    {
        noFlow.weight = 1.0f;
        flow.weight = 0.0f;
    }

    IEnumerator Flerp(bool lerpUp)
    {
        inFlow = playerMovement.inFlow;
        float lerpVal = lerpUp ? 0.0f : 1.0f;
        if (lerpUp)
        {
            while(lerpVal < 1.0f)
            {
                lerpVal += Time.deltaTime;
                noFlow.weight = Mathf.Lerp(1.0f, 0.0f, lerpVal);
                flow.weight = Mathf.Lerp(0.0f, 1.0f, lerpVal);
                gloabalLight.intensity = Mathf.Lerp(0.75f, 2.0f, lerpVal);
                yield return null;
            }
        }
        else
        {
            while(lerpVal > 0.0f)
            {
                lerpVal -= Time.deltaTime;
                noFlow.weight = Mathf.Lerp(1.0f, 0.0f, lerpVal);
                flow.weight = Mathf.Lerp(0.0f, 1.0f, lerpVal);
                gloabalLight.intensity = Mathf.Lerp(0.75f, 2.0f, lerpVal);
                yield return null;
            }
        }
    }
}
