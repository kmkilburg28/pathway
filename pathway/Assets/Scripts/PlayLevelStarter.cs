using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLevelStarter : MonoBehaviour {

	// Use this for initialization
	void Awake ()
    {
        GameController.gameController.SpawnPlayer();
	}
}
