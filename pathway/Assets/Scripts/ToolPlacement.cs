using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPlacement : MonoBehaviour
{
    MeshRenderer meshRenderer;

    public static bool isPlacing;

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        GameController.gameController.buildMode = true;
        Time.timeScale = 0.0f;
        GetComponent<Collider>().enabled = false;
        GameController.gameController.GetComponent<OutlineBox>().ShowOutline(this.gameObject);
        isPlacing = true;
    }

    // Update is called once per frame
    void Update()
    {
        GameController.gameController.GetComponent<OutlineBox>().SignalPlaceability();
        // Place tool
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (GameController.gameController.IsObjectPlaceable(this.gameObject))
            {
                EndMove();

                // Close this script
                this.GetComponent<ToolPlacement>().enabled = false;
            }
        }
        // Cancel tool
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            EndMove();
            Destroy(this.gameObject);
        }

        MoveObject();

    }


    public void EndMove()
    {
        GameController.gameController.buildMode = false;
        Time.timeScale = 1.0f;
        GameController.gameController.GetComponent<OutlineBox>().RemoveOutline();
        GetComponent<Collider>().enabled = true;
        isPlacing = false;
    }

    void MoveObject()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = Camera.main.transform.position.y;
        Vector3 followPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        float xAdjustment = 0;
        float zAdjustment = 0;


        // Tool center tracks mouse
        // Create x adjustment
        if (Mathf.RoundToInt(meshRenderer.bounds.size.x) % 2 != 0)
        {
            int multiplyFactor = (Mathf.RoundToInt(followPosition.x - Mathf.FloorToInt(followPosition.x)) == 0 ? 1 : -1);
            xAdjustment += (0.5f * multiplyFactor);
        }

        // Create z adjustment
        if (Mathf.RoundToInt(meshRenderer.bounds.size.z) % 2 != 0)
        {
            int multiplyFactor = (Mathf.RoundToInt(followPosition.z - Mathf.FloorToInt(followPosition.z)) == 0 ? 1 : -1);
            zAdjustment += (0.5f * multiplyFactor);
        }

        // Fit tool to grid
        transform.position = new Vector3(
            Mathf.Round(followPosition.x) + xAdjustment,
            transform.position.y,
            Mathf.Round(followPosition.z) + zAdjustment
        );
    }
}
