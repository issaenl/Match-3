using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle toggleMusic;
    public Slider sliderVolumeMusic;
    public Toggle toggleSFX;
    public Slider sliderVolumeSFX;

    private float musicVolume = 1f;
    private bool musicEnabled = true;
    private float sfxVolume = 1f;
    private bool sfxEnabled = true;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Load();
        UpdateVolumes();
    }

    public void SliderMusic()
    {
        musicVolume = sliderVolumeMusic.value;
        Save();
        UpdateToggles();
        UpdateVolumes();
    }

    public void ToggleMusic()
    {
        musicEnabled = toggleMusic.isOn;
        musicVolume = musicEnabled ? 1f : 0f;
        Save();
        UpdateVolumes();
    }

    public void SliderSFX()
    {
        sfxVolume = sliderVolumeSFX.value;
        Save();
        UpdateToggles();
        UpdateVolumes();
    }

    public void ToggleSFX()
    {
        sfxEnabled = toggleSFX.isOn;
        sfxVolume = sfxEnabled ? 1f : 0f;
        Save();
        UpdateVolumes();
    }

    private void UpdateToggles()
    {
        toggleMusic.isOn = musicVolume > 0f;
        toggleSFX.isOn = sfxVolume > 0f;
    }

    private void UpdateVolumes()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(musicVolume);
            SoundManager.Instance.SetSFXVolume(sfxVolume);
        }
        sliderVolumeMusic.value = musicVolume;
        toggleMusic.isOn = musicEnabled;
        sliderVolumeSFX.value = sfxVolume;
        toggleSFX.isOn = sfxEnabled;
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
    }

    private void Load()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
    }
}
