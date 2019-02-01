using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameController.gameController.MakeGrid();
        GameController.gameController.LoadLevelObstacles();

        if (GameController.gameController.levelMode == GameController.LevelMode.Constructing)
        {
            GameController.gameController.viewMode = GameController.ViewMode.Spectate;
            SceneManager.LoadScene("ConstructLevel");
        }
        else
        {
            GameController.gameController.viewMode = GameController.ViewMode.Tracking;
            SceneManager.LoadScene("PlayLevel");
        }
    }
}
