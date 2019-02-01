using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    public void SelectLevel()
    {
        string buttonText = transform.GetChild(transform.childCount - 1).GetComponent<Text>().text;
        int levelNum = int.Parse(buttonText[buttonText.Length - 1] + "");
        string levelName = EditLevelList.GetLevelNameAt(levelNum - 1);
        Debug.Log(levelNum + ": " + levelName);
        GameController.gameController.SetLevel(levelName);
        GameController.gameController.PlayLevel();
    }
}
