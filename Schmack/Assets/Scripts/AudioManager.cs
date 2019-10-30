using UnityEngine;

[System.Serializable]
public class Sound
{
    
    public string name;     //Name of sound
    public AudioClip clip;  //Audio File


    [Range(0f,1f)]          //Slider for volume
    public float volume = 0.7f;

    [Range(0.5f,1.5f)]      //Slider for pitch
    public float pitch = 1f;

    private AudioSource source; //reference to Audio Source
    
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;     //Sets Audio clip
    }

    //Method to play the audio
    public void Play()
    {
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    Sound[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
