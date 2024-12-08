using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score;
    public Image progressLine;
    private Board board;
    private GameData gameData;
    private int numberStars;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    public void SaveScore()
    {
        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }
            int currentStars = gameData.saveData.stars[board.level];
            if(numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;
            }
            gameData.Save();
        }
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        for(int i = 0; i < board.scoreGoals.Length; i++)
        {
            if(score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }
        UpdateLine();
    }

    public void UpdateLine()
    {
        if (board != null && progressLine != null)
        {
            int length = board.scoreGoals.Length;
            progressLine.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }
}
