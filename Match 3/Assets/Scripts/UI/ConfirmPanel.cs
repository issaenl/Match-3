using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ConfirmPanel : MonoBehaviour
{
    private GameData gameData;

    private int starsActive;

    public string levelToLoad;
    public Image[] stars;
    public TextMeshProUGUI highScoreText;
    public int level;
    private int highScore;
    // Start is called before the first frame update
    void OnEnable()
    {
        gameData = FindObjectOfType<GameData>();
        LoadData();
        ActiveStars();
        SetText();
    }

    void LoadData()
    {
        if(gameData != null)
        {
            starsActive = gameData.saveData.stars[level - 1];
            highScore = gameData.saveData.highScores[level - 1];
        }
    }

    void SetText()
    {
        highScoreText.text = highScore.ToString();
    }

    void ActiveStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }

    public void Play()
    {
        PlayerPrefs.SetInt("Current level", level - 1);
        SceneManager.LoadScene(levelToLoad);
    }
}
