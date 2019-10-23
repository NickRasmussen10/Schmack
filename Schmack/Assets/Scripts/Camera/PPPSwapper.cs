using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PPPSwapper : MonoBehaviour
{
    [SerializeField] PostProcessingProfile goodVibe;
    [SerializeField] PostProcessingProfile badVibe;

    bool vibing;
    PlayerMovement playerMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        vibing = playerMovement.GetVibing();
    }

    // Update is called once per frame
    void Update()
    {
        //if the vibe has changed
        if(vibing != playerMovement.GetVibing())
        {
            vibing = playerMovement.GetVibing();
            if (vibing)
            {
                GetComponent<PostProcessingBehaviour>().profile = goodVibe;
            }
            else
            {
                GetComponent<PostProcessingBehaviour>().profile = badVibe;
            }
        }
    }
}
