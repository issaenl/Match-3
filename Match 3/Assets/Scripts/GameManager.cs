using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}


public class GameManager : MonoBehaviour
{
    public EndGameRequirements requirements;
    public TextMeshProUGUI label;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public int currentCounterValue;
    private float timerSeconds;
    private Board board;
    private GoalManager goalManager;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        goalManager = FindObjectOfType<GoalManager>();
        SetupGame();
    }


    void SetupGame()
    {
        timerSeconds = 1;
        currentCounterValue = requirements.counterValue;
        SetupMovesTimeText();
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            SetupMovesTimeText();
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        WinPanel.SetActive(true);
        board.currentState = GameState.win;
        goalManager.SetupWinPanel();
        currentCounterValue = 0;
        SetupMovesTimeText();
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        LosePanel.SetActive(true);
        board.currentState = GameState.lose;
        goalManager.SetupLosePanel();
        currentCounterValue = 0;
        SetupMovesTimeText();
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    void SetupMovesTimeText()
    {
        string text = $"{currentCounterValue} {ChangeCase(currentCounterValue)}";
        if (requirements.gameType == GameType.Moves)
        {
            label.text = text;
        }
        else
        {
            label.text = currentCounterValue.ToString();
        }
    }

    string ChangeCase(int moves)
    {
        if (moves == 1)
        {
            return "ход";
        }
        else if (moves >= 2 && moves <= 4)
        {
            return "хода";
        }
        else
        {
            return "ходов";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
