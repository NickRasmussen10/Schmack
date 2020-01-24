using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPPSwapper : MonoBehaviour
{
    [SerializeField] PostProcessingProfile flow = null;
    [SerializeField] PostProcessingProfile noFlow = null;

    PostProcessingProfile ppp = null;

    bool inFlow;
    PlayerMovement playerMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        ppp = GetComponent<PostProcessingBehaviour>().profile;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        inFlow = playerMovement.inFlow;
    }

    // Update is called once per frame
    void Update()
    {
        //if the vibe has changed
        if (inFlow != playerMovement.inFlow)
        {
            inFlow = playerMovement.inFlow;
            if (inFlow)
            {
                //GetComponent<PostProcessingBehaviour>().profile = flow;
                StartCoroutine(Flerp(true));

            }
            else
            {
                StartCoroutine(Flerp(false));
                //GetComponent<PostProcessingBehaviour>().profile = noFlow;
            }
        }
    }

    IEnumerator Flerp(bool lerpUp)
    {
        Debug.Log("oh boi be flerppin");
        float lerpVal;
        if (lerpUp) lerpVal = 0.0f;
        else lerpVal = 1.0f;

        BloomModel.Settings bloom = ppp.bloom.settings;
        ColorGradingModel.Settings colorGrade = ppp.colorGrading.settings;
        ChromaticAberrationModel.Settings chromieAbs = ppp.chromaticAberration.settings;
        GrainModel.Settings grain = ppp.grain.settings;
        VignetteModel.Settings vigntte = ppp.vignette.settings;

        while ((lerpUp && lerpVal < 1.0f) || (!lerpUp && lerpVal > 0.0f))
        {
            if (lerpUp)
            {
                lerpVal += Time.deltaTime;
                if (lerpVal > 1.0f) lerpVal = 1.0f;
            }
            else
            {
                lerpVal -= Time.deltaTime * 4.0f;
                if (lerpVal < 0.0f) lerpVal = 0.0f;
            }

            //god has cursed me with my hubris
            //DepthOfFieldModel.Settings depthOfField = ppp.depthOfField.settings;
            //depthOfField.focusDistance = Mathf.Lerp(noFlow.depthOfField.settings.focusDistance,flow.depthOfField.settings.focusDistance, lerpVal);
            //depthOfField.aperture = Mathf.Lerp(noFlow.depthOfField.settings.aperture, flow.depthOfField.settings.aperture, lerpVal);
            //depthOfField.focalLength = Mathf.Lerp(noFlow.depthOfField.settings.focalLength, flow.depthOfField.settings.focalLength, lerpVal);
            //ppp.depthOfField.settings = depthOfField;

            bloom.bloom.intensity = Mathf.Lerp(noFlow.bloom.settings.bloom.intensity, flow.bloom.settings.bloom.intensity, lerpVal);
            bloom.bloom.threshold = Mathf.Lerp(noFlow.bloom.settings.bloom.threshold, flow.bloom.settings.bloom.threshold, lerpVal);
            bloom.bloom.softKnee = Mathf.Lerp(noFlow.bloom.settings.bloom.softKnee, flow.bloom.settings.bloom.softKnee, lerpVal);
            bloom.bloom.radius = Mathf.Lerp(noFlow.bloom.settings.bloom.radius, flow.bloom.settings.bloom.radius, lerpVal);
            bloom.lensDirt.intensity = Mathf.Lerp(noFlow.bloom.settings.lensDirt.intensity, flow.bloom.settings.lensDirt.intensity, lerpVal);
            ppp.bloom.settings = bloom;

            colorGrade.basic.postExposure = Mathf.Lerp(noFlow.colorGrading.settings.basic.postExposure, flow.colorGrading.settings.basic.postExposure, lerpVal);
            colorGrade.basic.temperature = Mathf.Lerp(noFlow.colorGrading.settings.basic.temperature, flow.colorGrading.settings.basic.temperature, lerpVal);
            colorGrade.basic.tint = Mathf.Lerp(noFlow.colorGrading.settings.basic.tint, flow.colorGrading.settings.basic.tint, lerpVal);
            colorGrade.basic.hueShift = Mathf.Lerp(noFlow.colorGrading.settings.basic.hueShift, flow.colorGrading.settings.basic.hueShift, lerpVal);
            colorGrade.basic.saturation = Mathf.Lerp(noFlow.colorGrading.settings.basic.saturation, flow.colorGrading.settings.basic.saturation, lerpVal);
            colorGrade.basic.contrast = Mathf.Lerp(noFlow.colorGrading.settings.basic.contrast, flow.colorGrading.settings.basic.contrast, lerpVal);
            ppp.colorGrading.settings = colorGrade;

            chromieAbs.intensity = Mathf.Lerp(noFlow.chromaticAberration.settings.intensity, flow.chromaticAberration.settings.intensity, lerpVal);
            ppp.chromaticAberration.settings = chromieAbs;

            grain.intensity = Mathf.Lerp(noFlow.grain.settings.intensity, flow.grain.settings.intensity, lerpVal);
            ppp.grain.settings = grain;

            vigntte.color.r = Mathf.Lerp(noFlow.vignette.settings.color.r, flow.vignette.settings.color.r, lerpVal);
            vigntte.color.g = Mathf.Lerp(noFlow.vignette.settings.color.g, flow.vignette.settings.color.g, lerpVal);
            vigntte.color.b = Mathf.Lerp(noFlow.vignette.settings.color.b, flow.vignette.settings.color.b, lerpVal);

            vigntte.intensity = Mathf.Lerp(noFlow.vignette.settings.intensity, flow.vignette.settings.intensity, lerpVal);
            vigntte.smoothness = Mathf.Lerp(noFlow.vignette.settings.smoothness, flow.vignette.settings.smoothness, lerpVal);
            vigntte.roundness = Mathf.Lerp(noFlow.vignette.settings.roundness, flow.vignette.settings.roundness, lerpVal);
            ppp.vignette.settings = vigntte;

            //LDR_LLL1_58
            yield return null;
        }
    }
}
