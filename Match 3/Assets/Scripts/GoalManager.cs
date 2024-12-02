using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSrite;
    public string matchValue;
}


public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab;
    public GameObject goalGamePrefab;
    public GameObject goalIntroParent;
    public GameObject goalGameParent;
    public GameObject winPanel;
    public GameObject losePanel;
    private GameManager gameManager;
    private bool hasShow = false;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        SetupIntroGoals();
    }

    void SetupIntroGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform);
            GoalPanel panel =  goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSrite;
            panel.thisString = levelGoals[i].numberNeeded.ToString();

            GameObject gameGoal = Instantiate(goalGamePrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisSprite = levelGoals[i].goalSrite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void SetupWinPanel()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject win = Instantiate(goalPrefab, winPanel.transform.position, Quaternion.identity);
            win.transform.SetParent(winPanel.transform);
            GoalPanel panel = win.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSrite;
            panel.thisString = levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
        }
    }

    public void SetupLosePanel()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject lose = Instantiate(goalPrefab, losePanel.transform.position, Quaternion.identity);
            lose.transform.SetParent(losePanel.transform);
            GoalPanel panel = lose.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSrite;
            panel.thisString = levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsComplited = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if(levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsComplited++;
                currentGoals[i].thisText.text = levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
        if(goalsComplited >= levelGoals.Length && !hasShow)
        {
            hasShow = true;
            if (gameManager != null)
            {
                gameManager.WinGame();
            }
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if(goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
