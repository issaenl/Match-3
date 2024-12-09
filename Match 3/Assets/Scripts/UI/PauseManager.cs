using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public bool paused = false;
    public Image music;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Image SFX;
    public Sprite SFXOnSprite;
    public Sprite SFXOffSprite;
    private Board board;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private bool musicEnabled = true;
    private bool sfxEnabled = true;

    void Start()
    {
        pausePanel.SetActive(false);
        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        Load();
        UpdateButtonImages();
    }

    void Update()
    {
        if (paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
        }
        if (!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
        }
    }

    public void PauseGame()
    {
        paused = !paused;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Level choose");
    }

    public void ChangeMusicVolume()
    {
        musicEnabled = !musicEnabled;
        UpdateSoundVolumes();
        UpdateButtonImages();
        Save();
    }

    public void ChangeSFXVolume()
    {
        sfxEnabled = !sfxEnabled;
        UpdateSoundVolumes();
        UpdateButtonImages();
        Save();
    }

    private void UpdateSoundVolumes()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicVolume(musicEnabled ? musicVolume : 0f);
            SoundManager.Instance.SetSFXVolume(sfxEnabled ? sfxVolume : 0f);
        }
    }


    private void UpdateButtonImages()
    {
        music.sprite = musicEnabled ? musicOnSprite : musicOffSprite;
        SFX.sprite = sfxEnabled ? SFXOnSprite : SFXOffSprite;
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
