using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public bool isActive;
    public Sprite activeSprite;
    public Sprite lockedSprite;
    private Image buttonImage;
    private Button myButton;
    public TextMeshProUGUI levelNumber;

    public Image[] stars;
    public int level;
    public GameObject confirmPanel;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        ActiveStars();
        ShowLevel();
        DecideSprite();
    }

    void ActiveStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }

    void DecideSprite()
    {
        if(isActive)
        {
            buttonImage.sprite = activeSprite;
            myButton.enabled = true;
            levelNumber.enabled = true;
        }
        else
        {
            buttonImage.sprite = lockedSprite;
            myButton.enabled = false;
            levelNumber.enabled = false;
        }
    }

    void ShowLevel()
    {
        levelNumber.text = level.ToString(); 
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConfirmPanel(int level)
    {
        confirmPanel.GetComponent<ConfirmPanel>().level = level;
        confirmPanel.SetActive(true);
    }
}
