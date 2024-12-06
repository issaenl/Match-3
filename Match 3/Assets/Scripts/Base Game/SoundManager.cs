using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource[] musicSources;
    public static SoundManager Instance;

    private void Awake() 
    { 
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        } 
        else 
        { 
            Destroy(gameObject); 
        } 
    }
    public void SetMusicVolume(float volume)
    {
        if (musicSources == null || musicSources.Length == 0)
        {
            return;
        }
        foreach (AudioSource source in musicSources)
        {
            source.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (destroyNoise == null || destroyNoise.Length == 0)
        {
            return;
        }
        foreach (var source in destroyNoise)
        {
            source.volume = volume;
        }
    }

    public void PlayRandomNoise() 
    { 
        int clipToPlay = Random.Range(0, destroyNoise.Length); 
        destroyNoise[clipToPlay].Play(); 
    }
}
