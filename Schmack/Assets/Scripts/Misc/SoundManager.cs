using UnityEngine;
using System;
using UnityEngine.Audio;

[System.Serializable] public class Sound
{
    public AudioClip clip;
    public string name;
    [Range(0.0f, 1.0f)] public float volume = 1.0f;
    [Range(0.1f, 3.0f)] public float pitch = 1.0f;
    public bool loop = false;

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
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
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

    public void Play(string name, float minPitch, float maxPitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.source)
        {
            s.source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            s.source.Play();
        }
    }

    public void PlayRandom(string[] names)
    {
        int index = UnityEngine.Random.Range(0, names.Length);
        Sound s = Array.Find(sounds, sound => sound.name == names[index]);
        if(s.source) s.source.Play();
    }

    public void PlayRandom(string[] names, float minPitch, float maxPitch)
    {
        int index = UnityEngine.Random.Range(0, names.Length);
        Sound s = Array.Find(sounds, sound => sound.name == names[index]);
        if (s.source)
        {
            s.source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            s.source.Play();
        }
    }

    public bool IsPlaying(string name)
    {
        return Array.Find(sounds, sound => sound.name == name).source.isPlaying;
    }

    public bool IsPlayingAny(string[] names)
    {
        foreach (string item in names)
        {
            if(Array.Find(sounds, sound => sound.name == item).source.isPlaying) return true;
        }
        return false;
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s.source) s.source.Stop();
    }
}
