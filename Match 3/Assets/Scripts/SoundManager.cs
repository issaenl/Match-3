using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource[] destroyNoise;

    public void PlayRandomNoise()
    {
        int clipToPaly = Random.Range(0, destroyNoise.Length);
        destroyNoise[clipToPaly].Play();
    }
}
