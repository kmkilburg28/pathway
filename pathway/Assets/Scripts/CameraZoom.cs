using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

    public float cameraZoomSpeed;
    public float startDistance;
    public float awayDistance;
    public float movementFactor;
    public bool recentering;

    GameObject player;
    Rigidbody playerRB;
    private float initialY;
    private float cameraTrackSpeed;

    // Use this for initialization
    void Start ()
    {
        transform.position = new Vector3(
            GameController.gameController.numOfCols / 2.0f,
            transform.position.y,
            -GameController.gameController.numOfRows / 2.0f + 1.0f
        );
        initialY = transform.position.y;
        if (GameController.gameController.viewMode == GameController.ViewMode.Tracking)
        {
            player = GameController.gameController.player;
            playerRB = player.GetComponent<Rigidbody>();
            transform.position = new Vector3(
                transform.position.x,
                player.transform.position.y + startDistance,
                transform.position.z
            );
            initialY = transform.position.y;
            cameraTrackSpeed = 2;
            recentering = true;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!GameController.gameController.buildMode)
        {
            if (GameController.gameController.viewMode == GameController.ViewMode.Spectate)
            {
                // User must click and drag in spectate mode to move camera
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    transform.position = new Vector3(
                        transform.position.x - (Input.GetAxis("Mouse X") * movementFactor * (transform.position.y / initialY)),
                        transform.position.y,
                        transform.position.z - (Input.GetAxis("Mouse Y") * movementFactor * (transform.position.y / initialY))
                    );
                }
                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Clamp(transform.position.y - Input.GetAxis("Mouse ScrollWheel"), 5f, 50f),
                    transform.position.z
                );
            }
            else if (recentering)
            {
                if (GameController.gameController.ballStatus == GameController.BallStatus.Moving)
                {
                    transform.position = new Vector3(
                        player.transform.position.x,
                        transform.position.y,
                        player.transform.position.z
                    );
                }
                else
                {
                    RecenterCamera();
                }
            }
            else
            {
                if (GameController.gameController.ballStatus == GameController.BallStatus.Moving)
                {
                    transform.position = new Vector3(
                        player.transform.position.x,
                        transform.position.y,
                        player.transform.position.z
                    );
                }
            }
        }
	}

    void RecenterCamera()
    {
        float changeX = (player.transform.position.x - transform.position.x) * Time.deltaTime * cameraTrackSpeed;
        float changeY = ((player.transform.position.y + awayDistance) - transform.position.y) * Time.deltaTime * cameraTrackSpeed;
        float changeZ = (player.transform.position.z - transform.position.z) * Time.deltaTime * cameraTrackSpeed;
        float EPSILON = 0.1f;
        if (Mathf.Abs(transform.position.x - player.transform.position.x) < EPSILON &&
            Mathf.Abs(transform.position.y - (player.transform.position.y + awayDistance)) < EPSILON &&
            Mathf.Abs(transform.position.z - player.transform.position.z) < EPSILON
           )
        {
            recentering = false;
        }
        else
        {
            transform.position = new Vector3(
                transform.position.x + changeX,
                transform.position.y + changeY,
                transform.position.z + changeZ
            );
        }
    }

    public void TriggerRecenter()
    {
        recentering = true;
    }
}
