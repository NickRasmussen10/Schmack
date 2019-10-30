using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSounds : MonoBehaviour
{

    public AudioManager audioMan;
    // Start is called before the first frame update
    void Awake()
    {
        if(audioMan==null)
        {
            Debug.LogError("No audio manager found in scene");
        }
        audioMan = AudioManager.instance;
    }
    private void Start()
    {
        audioMan.PlaySound("BGMusic");
        audioMan.PlaySound("AmbientSound");
    }
}
