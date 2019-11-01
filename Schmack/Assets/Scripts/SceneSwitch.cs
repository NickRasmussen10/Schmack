using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
        if(sceneNumber==1)
        {
            GameObject.FindGameObjectWithTag("AudioMan").GetComponent<AudioManager>().PlaySound("Ambient");
            GameObject.FindGameObjectWithTag("AudioMan").GetComponent<AudioManager>().PlaySound("BG");

        }
        if (sceneNumber == 2)
        {
            GameObject.FindGameObjectWithTag("AudioMan").GetComponent<AudioManager>().StopSounds("BG");
            GameObject.FindGameObjectWithTag("AudioMan").GetComponent<AudioManager>().StopSounds("Ambient");
            GameObject.FindGameObjectWithTag("AudioMan").GetComponent<AudioManager>().PlaySound("Falling");

        }
    }
}
