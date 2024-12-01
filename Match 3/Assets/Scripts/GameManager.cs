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
    public int currentCounterValue;
    private float timerSeconds;

    // Start is called before the first frame update
    void Start()
    {
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

        currentCounterValue--;
        SetupMovesTimeText();
        if (currentCounterValue <= 0)
        {
            Debug.Log("End");
            currentCounterValue = 0;
            SetupMovesTimeText();
        }
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
            return "���";
        }
        else if (moves >= 2 && moves <= 4)
        {
            return "����";
        }
        else
        {
            return "�����";
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