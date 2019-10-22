using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingTest : MonoBehaviour
{
    [SerializeField] PostProcessingProfile flow;
    [SerializeField] PostProcessingProfile noFlow;
    [SerializeField] ParticleSystem rain;

    bool inFlow;
    // Start is called before the first frame update
    void Start()
    {
        inFlow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            inFlow = !inFlow;
            if (inFlow)
            {
                gameObject.GetComponent<PostProcessingBehaviour>().profile = flow;
                rain.enableEmission = false;
            }
            else
            {
                gameObject.GetComponent<PostProcessingBehaviour>().profile = noFlow;
                rain.enableEmission = true;
            }
        }
    }
}
