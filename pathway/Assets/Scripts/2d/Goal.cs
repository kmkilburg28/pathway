/*
 Created by Kaden Kilburg
 10/5/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    GameObject winText;
    
	// Use this for initialization
	void Start () {
        winText = GameObject.Find("winText");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.Equals("projectile"))
        {
            winText.SetActive(true);
        }
    }
    */
}
