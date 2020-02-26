using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    PostProcessVolume ppv_flow = null; //post processing volume for flow
    PostProcessVolume ppv_noFlow = null; //post processing volume for reality
    PostProcessVolume ppv_playerDamage; //post processing volume for player damage
    UnityEngine.Experimental.Rendering.LWRP.Light2D globalLight = null; //scene's global light

    //player scripts
    PlayerMovement playerMovement;
    Player player;

    CinemachineFramingTransposer framingTransposer;

    Image colorFade;

    bool inFlow;
    float flerpVal = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //setup post processing volumes, must be in flow, no flow, player damage order
        PostProcessVolume[] volumes = gameObject.GetComponentsInChildren<PostProcessVolume>();
        ppv_flow = volumes[0];
        ppv_noFlow = volumes[1];
        ppv_playerDamage = volumes[2];

        //get global light
        globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<UnityEngine.Experimental.Rendering.LWRP.Light2D>();

        //get player script references
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerMovement = player.gameObject.GetComponent<PlayerMovement>();

        //get framing transposer
        framingTransposer = gameObject.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();

        colorFade = GameObject.Find("ColorFade").GetComponent<Image>();
        colorFade.color = new Color(colorFade.color.r, colorFade.color.g, colorFade.color.b, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        LeadPlayer();

        //if flow has changed, update camera settings based on change
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
        StopCoroutine(DisplayDamage(time));
        StartCoroutine(DisplayDamage(time));
    }

    void CallDisplayDeath(Vector3 playerPos)
    {
        StartCoroutine(DisplayDeath(playerPos));
    }

    IEnumerator DisplayDamage(float time)
    {
        float weight = ppv_playerDamage.weight;
        float maxWeight = weight + 0.3f;
        if (weight > 1.0f) weight = 1.0f;
        while(weight < maxWeight)
        {
            weight += Time.deltaTime;
            if (weight > maxWeight) weight = maxWeight;

            ppv_playerDamage.weight = weight;
            yield return null;
        }

        yield return new WaitForSeconds(time);
        
        while(weight > 0.0f)
        {
            weight -= Time.deltaTime;
            if (weight < 0.0f) weight = 0.0f;
            ppv_playerDamage.weight = weight;
            yield return null;
        }
    }

    IEnumerator DisplayDeath(Vector3 playerPos)
    {
        float lerpVal = 0.0f;
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();

        Vector3 targetPos = playerPos;
        Vector3 origin = Camera.main.transform.position;
        Vector3 position = Camera.main.transform.position;

        Color fade = colorFade.color;

        while(lerpVal < 1.0f)
        {
            lerpVal += Time.deltaTime;
            if (lerpVal > 1.0f) lerpVal = 1.0f;

            vcam.m_Lens.OrthographicSize = Mathf.Lerp(8.5f, 6.0f, lerpVal);
            position.x = Mathf.Lerp(origin.x, targetPos.x, lerpVal);
            position.y = Mathf.Lerp(origin.y, targetPos.y, lerpVal);
            Camera.main.transform.position = position;

            fade.a = Mathf.Lerp(0.0f, 1.0f, lerpVal);
            colorFade.color = fade;
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        FindObjectOfType<SceneSwitch>().LoadScene(2);
    }

    IEnumerator FlerpToFlow()
    {
        inFlow = true;
        flerpVal = ppv_flow.weight;
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        while(flerpVal < 1.0f)
        {
            flerpVal += Time.deltaTime * 4.0f;
            if (flerpVal > 1.0f) flerpVal = 1.0f;

            ppv_noFlow.weight = Mathf.Lerp(1.0f, 0.0f, flerpVal);
            ppv_flow.weight = Mathf.Lerp(0.0f, 1.0f, flerpVal);
            globalLight.intensity = Mathf.Lerp(0.75f, 2.0f, flerpVal);

            vcam.m_Lens.OrthographicSize = Mathf.Lerp(8.5f, 10.0f, flerpVal);

            yield return null;
        }
    }

    IEnumerator FlerpToReality()
    {
        inFlow = false;
        flerpVal = ppv_noFlow.weight;
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        while(flerpVal < 1.0f)
        {
            flerpVal += Time.deltaTime * 4.0f;
            if (flerpVal > 1.0f) flerpVal = 1.0f;

            ppv_noFlow.weight = Mathf.Lerp(0.0f, 1.0f, flerpVal);
            ppv_flow.weight = Mathf.Lerp(1.0f, 0.0f, flerpVal);
            globalLight.intensity = Mathf.Lerp(2.0f, 0.75f, flerpVal);

            vcam.m_Lens.OrthographicSize = Mathf.Lerp(10.0f, 8.5f, flerpVal);

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
