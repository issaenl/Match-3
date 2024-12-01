using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public Animator settingsAnim;

    public void Exit()
    {
        if (settingsAnim != null)
        {
            settingsAnim.SetBool("Out", true);
            settingsAnim.SetBool("In", false);
        }
    }

    public void SettingsOpen()
    {
        if (settingsAnim != null)
        {
            settingsAnim.SetBool("In", true);
            settingsAnim.SetBool("Out", false);
        }
    }
}
