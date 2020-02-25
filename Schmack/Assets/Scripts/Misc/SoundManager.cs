using UnityEngine;
using System;
using UnityEngine.Audio;

[System.Serializable] public class Sound
{
    public AudioClip clip;
    public string name;
    [Range(0.0f, 1.0f)] public float volume;
    [Range(0.1f, 3.0f)] public float pitch;

    [HideInInspector] public AudioSource source;
}


public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.source) s.source.Play();
    }
}
