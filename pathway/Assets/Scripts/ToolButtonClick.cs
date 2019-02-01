using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolButtonClick : MonoBehaviour {

    public GameObject toolPrefab;

    public void PlaceTool()
    {
        if (ToolPlacement.isPlacing)
        {
            GameController.gameController.focusObject.GetComponent<ToolPlacement>().EndMove();
            Destroy(GameController.gameController.focusObject);
        }
        GameObject placement = Instantiate(toolPrefab);
        placement.AddComponent<ToolPlacement>();
    }
}
