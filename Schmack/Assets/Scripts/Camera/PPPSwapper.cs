using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPPSwapper : MonoBehaviour
{
    [SerializeField] PostProcessingProfile flow = null;
    [SerializeField] PostProcessingProfile noFlow = null;

    bool vibing;
    PlayerMovement playerMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        vibing = playerMovement.vibing;
    }

    // Update is called once per frame
    void Update()
    {
        //if the vibe has changed
        if (vibing != playerMovement.vibing)
        {
            vibing = playerMovement.vibing;
            if (vibing)
            {
                GetComponent<PostProcessingBehaviour>().profile = flow;

            }
            else
            {
                GetComponent<PostProcessingBehaviour>().profile = noFlow;
            }
        }
    }
}
