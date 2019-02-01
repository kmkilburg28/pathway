using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerIndicator : MonoBehaviour {

    Player playerScript;
    
    RectTransform rectTransform;

    RectTransform powerBarRectTransform;

    float changeInXPerForce;

    float initialPosition;


	// Use this for initialization
	void Start () {
        rectTransform = GetComponent<RectTransform>();
        powerBarRectTransform = transform.parent.GetComponent<RectTransform>();
        playerScript = GameController.gameController.player.GetComponent<Player>();

        Image powerBarImage = transform.parent.gameObject.GetComponent<Image>();
        initialPosition = powerBarImage.transform.position.x - ((powerBarRectTransform.offsetMax.x - powerBarRectTransform.offsetMin.x) / 2);
        changeInXPerForce = (powerBarRectTransform.offsetMax.x - powerBarRectTransform.offsetMin.x) / playerScript.maxForce;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameController.gameController.ballStatus == GameController.BallStatus.Charging)
        {
            float changeInX = playerScript.totalForce * changeInXPerForce;
            rectTransform.position = new Vector3(
                initialPosition + changeInX,
                transform.position.y,
                transform.position.z
            );
        }
    }
}
