using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeArrow : MonoBehaviour {

    GameObject player;
    Player playerScript;
    float maxForce;

    Vector3 rotationPoint;
    
    Vector3 mousePosition;

    MeshRenderer meshRenderer;


    // Use this for initialization
    void Start ()
    {
        player = GameController.gameController.player;
        playerScript = player.GetComponent<Player>();
        maxForce = playerScript.maxForce;

        meshRenderer = GetComponent<MeshRenderer>();
        transform.position = player.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameController.gameController.ballStatus == GameController.BallStatus.Charging)
        {
            meshRenderer.enabled = true;
            //rotationPoint = camera.WorldToScreenPoint(player.transform.position);
            mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.transform.position.y;
            float angle = GetFiringAngle();
            Quaternion rotation = Quaternion.Euler(0f, 180f-angle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);
            //transform.Rotate(0f, GetRotation(), 0f, Space.World);
            SetPosition(Mathf.Deg2Rad * angle);
            //SetScale();

            //rectTransform.pivot.Set(rotationPoint.x, rotationPoint.y);
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }

    void SetPosition(float givenAngleInRads)
    {
        float maxPosition = 2f;
        float xAdjustment = Mathf.Cos(givenAngleInRads) * maxPosition;
        float zAdjustment = Mathf.Sin(givenAngleInRads) * maxPosition;
        transform.position = new Vector3(
            player.transform.position.x + xAdjustment,
            player.transform.position.y,
            player.transform.position.z + zAdjustment
        );
    }

    void SetScale()
    {
        float newScale = ((playerScript.totalForce / maxForce) * 0.5f) + 0.5f;
        Debug.Log(newScale);
        transform.localScale = new Vector3(
            newScale,
            newScale,
            newScale
        );
    }

    private float GetFiringAngle()
    {
        float angle;
        Vector2 localPos = Camera.main.WorldToScreenPoint(player.transform.position);
        float distanceX = localPos.x - mousePosition.x;
        float distanceY = localPos.y - mousePosition.y;
        angle = Mathf.Rad2Deg * Mathf.Atan(distanceY / distanceX);
        if (mousePosition.x < localPos.x)
        {
            angle += 180;
        }
        return angle + 180;
    }
}
