using UnityEngine;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{
    public InputField[] passwordFields;
    public Button[] eyeButtons;
    public Sprite eyeOpenSprite;
    public Sprite eyeClosedSprite;

    void Start()
    {
        for (int i = 0; i < eyeButtons.Length; i++)
        {
            int index = i;
            eyeButtons[i].onClick.AddListener(() => ShowHidePassword(index));
        }
    }

    void ShowHidePassword(int index)
    {
        if (passwordFields[index].contentType == InputField.ContentType.Password)
        {
            passwordFields[index].contentType = InputField.ContentType.Standard;
            eyeButtons[index].image.sprite = eyeOpenSprite;
        }
        else
        {
            passwordFields[index].contentType = InputField.ContentType.Password;
            eyeButtons[index].image.sprite = eyeClosedSprite;
        }
        passwordFields[index].ForceLabelUpdate();
    }
}

