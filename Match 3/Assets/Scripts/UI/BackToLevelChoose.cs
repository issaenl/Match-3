using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLevelChoose : MonoBehaviour
{
    public string sceneToLoad;
    private GameData gameData;
    private Board board;

    public void WinOK()
    {
        if (gameData != null)
        {
            if(board.level + 1 < gameData.saveData.isActive.Length)
            {
                gameData.saveData.isActive[board.level + 1] = true;
                gameData.Save();
            }
            else
            {
                gameData.Save();
            }
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoseOK()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
    }
}
