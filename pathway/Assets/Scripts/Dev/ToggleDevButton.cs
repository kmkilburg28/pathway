using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleDevButton : MonoBehaviour {

    public void ToggleDevMode()
    {
        GameController.gameController.developerMode = !GameController.gameController.developerMode;
        ChangeText();
    }

    void ChangeText()
    {
        Text buttonText = transform.GetChild(0).GetComponent<Text>();
        buttonText.text = "To ";
        if (GameController.gameController.developerMode)
        {
            buttonText.text += "PlayMode";
        }
        else
        {
            buttonText.text += "DevMode";
        }
    }
}
