using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateToolBar : MonoBehaviour {

    public GameObject prefab;

    private int numberToCreate;
    private int numberToCreateConstructs;

	// Use this for initialization
	void Start () {
        numberToCreate = GameController.gameController.toolList.Length;
        numberToCreateConstructs = GameController.gameController.constructToolsList.Length;
        Populate();
	}
	
	void Populate()
    {
        GameObject toolPrefab;

        if (GameController.gameController.viewMode == GameController.ViewMode.Spectate)
        {
            for (int i = 0; i < numberToCreateConstructs; i++)
            {
                toolPrefab = GameController.gameController.constructToolsList[i];
                CreateCell(toolPrefab);
            }
        }
        for (int i = 0; i < numberToCreate; i++)
        {
            toolPrefab = GameController.gameController.toolList[i];
            CreateCell(toolPrefab);
        }
    }

    void CreateCell(GameObject toolPrefab)
    {
        GameObject cell;
        cell = Instantiate(prefab, transform);
        Sprite toolImage = toolPrefab.GetComponent<Image>().sprite;
        cell.transform.GetChild(0).GetComponent<Image>().sprite = toolImage;
        cell.transform.GetChild(cell.transform.childCount - 2).GetComponent<Text>().text = FormatName(toolPrefab.name);
        cell.transform.GetChild(cell.transform.childCount - 1).GetComponent<ToolButtonClick>().toolPrefab = toolPrefab;
    }

    string FormatName(string savedName)
    {
        string retName = "" + char.ToUpper(savedName[0]);
        for (int i = 1; i < savedName.Length; i++)
        {
            if (char.IsUpper(savedName[i]))
            {
                retName += " ";
            }
            retName += savedName[i];
        }
        return retName;
    }
}
