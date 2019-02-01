using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateLevelSelection : MonoBehaviour {

    public GameObject cellPrefab;

    private int numberToCreate;

    // Use this for initialization
    void Start () {
        numberToCreate = EditLevelList.GetLevelCount();
        Populate();
	}

    void Populate()
    {
        GameObject newCell;

        for (int i = 0; i < numberToCreate; i++)
        {
            newCell = Instantiate(cellPrefab, transform);
            newCell.transform.GetChild(newCell.transform.childCount - 1).GetComponent<Text>().text = "Level " + (i + 1);
        }
    }
}
