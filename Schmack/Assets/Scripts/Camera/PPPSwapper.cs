using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPPSwapper : MonoBehaviour
{
    [SerializeField] PostProcessingProfile flow = null;
    [SerializeField] PostProcessingProfile noFlow = null;

    bool inFlow;
    PlayerMovement playerMovement;
    
    // Start is called before the first frame update
    void Start()
    {
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
                GetComponent<PostProcessingBehaviour>().profile = flow;

            }
            else
            {
                GetComponent<PostProcessingBehaviour>().profile = noFlow;
            }
        }
    }

    IEnumerator Flerp()
    {
        yield return null;
    }
}
