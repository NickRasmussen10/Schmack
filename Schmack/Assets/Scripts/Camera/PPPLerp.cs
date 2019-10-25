using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPPLerp : MonoBehaviour
{
    [SerializeField] PostProcessingProfile flow;
    [SerializeField] PostProcessingProfile noFlow;
    PostProcessingProfile PPP;
    float lerpVal; //0 = noFlow, 1 = flow

    PlayerMovement pMovement;

    // Start is called before the first frame update
    void Start()
    {
        lerpVal = 0.0f;
        pMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pMovement.vibing && lerpVal < 1)
        {
            //lerp up to flow PPP
        }
        else if(!pMovement.vibing && lerpVal > 0)
        {
            //lerp down to noFlow PPP
        }
    }
}
