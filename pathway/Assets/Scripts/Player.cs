using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float strength;
    
    public Vector3 forceXZ;
    
    public static Vector3 playerSpawnLocation;

    public float totalForce;

    public float maxForce;

    public bool forceIncreasing;

    public float gravityAdjustment;

    public float stoppedSpeed;
    
    private bool mouseDownTriggered;

    MeshRenderer meshRenderer;
    
    Rigidbody thisRB;
  
    // Use this for initialization
    void Start ()
    {
        GameController.gameController.ballStatus = GameController.BallStatus.Rest;
        totalForce = 0;
        forceIncreasing = true;
        thisRB = GetComponent<Rigidbody>();
        thisRB.sleepThreshold = 5f;
        meshRenderer = GetComponent<MeshRenderer>();
        mouseDownTriggered = false;
    }
	
    // Update is called once per frame
	void Update ()
    {
        if (GameController.gameController.viewMode == GameController.ViewMode.Tracking && !GameController.gameController.buildMode)
        {
            if (GameController.gameController.ballStatus == GameController.BallStatus.Rest ||
                GameController.gameController.ballStatus == GameController.BallStatus.Charging)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    totalForce = 0;
                    GameController.gameController.ActivateChargeMode();
                    mouseDownTriggered = true;
                }
                else if (mouseDownTriggered)
                {
                    if (Input.GetKey(KeyCode.Mouse1))
                    {
                        // Determine force
                        totalForce = Mathf.Clamp(DetermineTotalDistance() * strength, 0f, maxForce);
                    }
                    if (Input.GetKeyUp(KeyCode.Mouse1))
                    {
                        thisRB.WakeUp();
                        SetForceXZ();
                        thisRB.AddForceAtPosition(
                            forceXZ,
                            new Vector3 (transform.position.x, transform.position.y + meshRenderer.bounds.extents.y/2, transform.position.z),
                            ForceMode.Impulse
                        );
                        GameController.gameController.ReleaseChargeMode();
                        mouseDownTriggered = false;
                    }
                }
            }
            if (thisRB.IsSleeping() &&
                GameController.gameController.ballStatus == GameController.BallStatus.Moving)
            {
                GameController.gameController.ballStatus = GameController.BallStatus.Rest;
            }
        }
	}

    private void FixedUpdate()
    {
        // If player falls off
        if (transform.position.y < -1f)
        {
            GameController.gameController.RespawnPlayer();
        }
        // Add gravity if not on a stable surface
        if (Mathf.Abs(thisRB.velocity.y) > 0.01f)
        {
            thisRB.AddForce(0f, gravityAdjustment, 0f);
        }
    }


    private void SetForceXZ()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouthPosition = Camera.main.WorldToScreenPoint(transform.position);
        float distanceX = mousePosition.x - mouthPosition.x;
        float distanceY = mousePosition.y - mouthPosition.y;
        float angle = Mathf.Atan(distanceY / distanceX) + Mathf.PI;
        float forceX = totalForce * Mathf.Cos(angle);
        float forceZ = totalForce * Mathf.Sin(angle);
        if (distanceX < 0)
        {
            forceX *= -1f;
            forceZ *= -1f;
        }
        forceXZ.Set(forceX, 0f, forceZ);
    }

    public float DetermineTotalDistance()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        // √((x2-x1)^2 + (y2-y1)^2)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 playerPos = meshRenderer.bounds.ClosestPoint(mousePos);
        float xSquare = Mathf.Pow((mousePos.x - playerPos.x), 2);
        float zSquare = Mathf.Pow((mousePos.z - playerPos.z), 2);
        float distance = Mathf.Sqrt(xSquare + zSquare);
        return distance;
    }
}
