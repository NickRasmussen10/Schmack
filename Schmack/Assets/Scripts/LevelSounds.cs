using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSounds : MonoBehaviour
{

    public AudioManager audioMan;
    // Start is called before the first frame update
    void Start()
    {
        if(audioMan==null)
        {
            Debug.LogError("No audio manager found in scene");
        }
        audioMan = AudioManager.instance;

        audioMan.PlaySound("BG");
        audioMan.PlaySound("Ambient");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
