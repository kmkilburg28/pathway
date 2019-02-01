using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameController.gameController.SetLevel("titleScreen");
        GameController.gameController.MakeGrid();
        GameController.gameController.LoadLevelObstacles();

        SceneManager.LoadScene("TitleScreen");
    }
}
