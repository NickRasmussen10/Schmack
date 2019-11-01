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

    [Range(0f,0.5f)]        //Slider for volume randomization
    public float volRand = 0.1f;   //Random Volume multiplyer

    [Range(0f,0.5f)]        //Slider for pitch randomization
    public float pitchRand = 0.1f;  //Random Pitch multiplyer

    public bool loop = false;

    private AudioSource source; //reference to Audio Source
    
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;     //Sets Audio clip
        source.loop = loop;     //Sets loop value
    }

    //Method to play the audio
    public void Play()
    {
        source.volume = volume * (1 + Random.Range(-volRand / 2f, volRand / 2f));
        source.pitch = pitch * (1 + Random.Range(-pitchRand / 2f, pitchRand / 2f));
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    Sound[] sounds;

    private void Awake()
    {
        if(instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<sounds.Length;i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
        PlaySound("BG");
    }

    public void PlaySound(string name)
    {
        for(int i=0; i<sounds.Length;i++)
        {
            if(sounds[i].name==name)
            {
                sounds[i].Play();
                return;
            }
        }
        Debug.LogWarning("AudioManager: Sound not found in list: " + name);
        
    }

    public void StopSounds(string name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                sounds[i].Stop();
                return;
            }
        }
    }


}
