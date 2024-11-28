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

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
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
