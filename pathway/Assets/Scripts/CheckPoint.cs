using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

    private bool hasBeenTriggered;

    private void Start()
    {
        hasBeenTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !hasBeenTriggered)
        {
            GameController.gameController.SetSpawnLocation(transform.position);
            hasBeenTriggered = true;
        }
    }
}
