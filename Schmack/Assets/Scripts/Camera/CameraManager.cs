using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    PostProcessVolume ppv_flow = null;
    PostProcessVolume ppv_noFlow = null;
    PostProcessVolume ppv_playerDamage;
    UnityEngine.Experimental.Rendering.LWRP.Light2D globalLight = null;

    PlayerMovement playerMovement;
    Player player;

    CinemachineFramingTransposer framingTransposer;

    bool inFlow;
    float lerpVal = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        PostProcessVolume[] volumes = gameObject.GetComponentsInChildren<PostProcessVolume>();
        ppv_flow = volumes[0];
        ppv_noFlow = volumes[1];
        ppv_playerDamage = volumes[2];

        globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<UnityEngine.Experimental.Rendering.LWRP.Light2D>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerMovement = player.gameObject.GetComponent<PlayerMovement>();

        framingTransposer = gameObject.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        LeadPlayer();

        if(playerMovement.inFlow && !inFlow)
        {
            StartCoroutine(FlerpToFlow());
        }
        else if(!playerMovement.inFlow && inFlow)
        {
            StartCoroutine(FlerpToReality());
        }
    }

    void LeadPlayer()
    {
        //update virtual camera's y bias and screen position to lead the player downwards
        framingTransposer.m_BiasY = Mathf.Clamp(playerMovement.GetVelocity().y * 0.01f, -.5f, .5f);
        framingTransposer.m_ScreenY = Mathf.Lerp(.6f, .3f, -(playerMovement.GetVelocity().y * 0.1f));
    }

    void CallDisplayDamage(float time)
    {
        Debug.Log("I'll be shocked if this works");
        StartCoroutine(DisplayDamage(time));
    }

    void CallDisplayDeath()
    {

    }

    IEnumerator DisplayDamage(float time)
    {
        float lerpVal = ppv_playerDamage.weight; //could possibly change this to player damamge's vignette intensity
        while(lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 3;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            ppv_playerDamage.weight = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            yield return null;
        }

        yield return new WaitForSeconds(time);
        
        while(lerpVal > 0.0f)
        {
            lerpVal -= Time.deltaTime;
            if (lerpVal < 0.0f) lerpVal = 0.0f;
            ppv_playerDamage.weight = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            yield return null;
        }
    }

    

    IEnumerator FlerpToFlow()
    {
        inFlow = true;
        lerpVal = ppv_flow.weight;
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        while(lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 4.0f;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            ppv_noFlow.weight = Mathf.Lerp(1.0f, 0.0f, lerpVal);
            ppv_flow.weight = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            globalLight.intensity = Mathf.Lerp(0.75f, 2.0f, lerpVal);

            vcam.m_Lens.OrthographicSize = Mathf.Lerp(8.5f, 10.0f, lerpVal);

            yield return null;
        }
    }

    IEnumerator FlerpToReality()
    {
        inFlow = false;
        lerpVal = ppv_noFlow.weight;
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        while(lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime * 4.0f;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            ppv_noFlow.weight = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            ppv_flow.weight = Mathf.Lerp(1.0f, 0.0f, lerpVal);
            globalLight.intensity = Mathf.Lerp(2.0f, 0.75f, lerpVal);

            vcam.m_Lens.OrthographicSize = Mathf.Lerp(10.0f, 8.5f, lerpVal);

            yield return null;
        }
    }



    private void OnApplicationQuit()
    {
        ppv_noFlow.weight = 1.0f;
        ppv_flow.weight = 0.0f;
        ppv_playerDamage.weight = 0.0f;
        globalLight.intensity = 0.75f;
    }
}
