using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutlineBox : MonoBehaviour {

    public Material faceMaterial;

    GameObject[] facePool;

    GameObject[] edgePool;

    public bool objectHighlighted;


    // Called in GameController
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Scene loadedLevel = SceneManager.GetActiveScene();
        if (loadedLevel.name.Equals("PlayLevel") || loadedLevel.name.Equals("ConstructLevel"))
        {
            CreateFaces();
            CreateEdgeLines();
        }
    }

    private void Update()
    {
        if (objectHighlighted)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                GameObject parent = facePool[0].transform.parent.gameObject;
                parent.transform.Rotate(0f, 90f, 0f);

                float xAdjustment = 0;
                float zAdjustment = 0;
                MeshRenderer meshRenderer = parent.GetComponent<MeshRenderer>();
                // Create x adjustment
                if (Mathf.RoundToInt(meshRenderer.bounds.size.x) % 2 != 0)
                {
                    int multiplyFactor = (Mathf.RoundToInt(parent.transform.position.x - Mathf.FloorToInt(parent.transform.position.x)) == 0 ? 1 : -1);
                    xAdjustment += (0.5f * multiplyFactor);
                }

                // Create z adjustment
                if (Mathf.RoundToInt(meshRenderer.bounds.size.z) % 2 != 0)
                {
                    int multiplyFactor = (Mathf.RoundToInt(parent.transform.position.z - Mathf.FloorToInt(parent.transform.position.z)) == 0 ? 1 : -1);
                    zAdjustment += (0.5f * multiplyFactor);
                }
                parent.transform.position = new Vector3(
                    Mathf.Round(parent.transform.position.x) + xAdjustment,
                    parent.transform.position.y,
                    Mathf.Round(parent.transform.position.z) + zAdjustment
                );
                AssignOutlineBox(parent);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                GameObject parent = facePool[0].transform.parent.gameObject;
                RemoveParent();
                DeactivateFacePool();
                DeactivateEdgePool();
                Destroy(parent);
            }
            if (ResizeObject.alreadyAdjusting)
            {
                ResizeEdges();
            }
        }
    }

    public void SignalPlaceability()
    {
        Color edgeColor;
        Color faceColor = Color.clear;
        if (!ResizeObject.alreadyAdjusting && !ToolPlacement.isPlacing)
        {
            edgeColor = new Color(0, 0, 0, 0.5f);
        }
        else
        {
            if (facePool[0].transform.parent != null && GameController.gameController.IsObjectPlaceable(facePool[0].transform.parent.gameObject))
            {
                Color green = new Color(0, 1, 0, 0.5f);
                edgeColor = green;
            }
            else
            {
                Color edgeRed = new Color(1, 0, 0, 0.5f);
                edgeColor = edgeRed;
                Color faceRed = new Color(1, 0, 0, 0.2f);
                faceColor = faceRed;
            }
        }

        for (int i = 0; i < facePool.Length; i++)
        {
            facePool[i].GetComponent<MeshRenderer>().material.color = faceColor;
        }
        for (int i = 0; i < edgePool.Length; i++)
        {
            edgePool[i].GetComponent<MeshRenderer>().material.color = edgeColor;
        }
    }

    public void ShowOutline(GameObject givenGameObject)
    {
        objectHighlighted = true;
        AssignOutlineBox(givenGameObject);
        Time.timeScale = 0.0f;
        SignalPlaceability();
    }

    void AssignOutlineBox(GameObject givenGameObject)
    {
        RemoveParent();
        ResetFaceRotations();
        PlaceFaces(givenGameObject);
        ResetEdgeRotations();
        PlaceEdges(givenGameObject);

        SetParent(givenGameObject);
        ActivateFacePool();

        GameController.gameController.focusObject = givenGameObject;
    }

    public void RemoveOutline()
    {
        if (facePool[0].transform.parent != null)
        {
            facePool[0].transform.parent.GetComponent<Collider>().enabled = true;
            objectHighlighted = false;
            Time.timeScale = 1.0f;
            DeactivateFacePool();
            DeactivateEdgePool();
            RemoveParent();
        }
    }

    void CreateFaces()
    {
        // Create and format an instance of a face
        GameObject faceInstance = GameObject.CreatePrimitive(PrimitiveType.Plane);
        faceInstance.GetComponent<MeshCollider>().convex = true;
        faceInstance.GetComponent<MeshCollider>().isTrigger = true;
        faceInstance.GetComponent<MeshRenderer>().material = faceMaterial;
        faceInstance.GetComponent<MeshRenderer>().material.color = Color.clear;
        faceInstance.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        faceInstance.AddComponent<ResizeObject>();

        // Generate a face pool
        facePool = new GameObject[5];
        for (int i = 0; i < facePool.Length; i++)
        {
            facePool[i] = Instantiate(faceInstance);
            facePool[i].SetActive(false);
        }

        // Top
        facePool[0].name = "faceTop";
        // North
        facePool[1].name = "faceNorth";
        // South
        facePool[2].name = "faceSouth";
        // East
        facePool[3].name = "faceEast";
        // West
        facePool[4].name = "faceWest";

        // Destroy cube instance
        Destroy(faceInstance);
    }

    void ResetFaceRotations()
    {
        // Top
        facePool[0].transform.rotation = Quaternion.Euler(Vector3.zero);

        // North
        facePool[1].transform.rotation = Quaternion.Euler(
            90f,
            0f,
            0f
        );

        // South
        facePool[2].transform.rotation = Quaternion.Euler(
            -90f,
            0f,
            0f
        );
        // East
        facePool[3].transform.rotation = Quaternion.Euler(
            0f,
            0f,
            -90f
        );
        // West
        facePool[4].transform.rotation = Quaternion.Euler(
            0f,
            0f,
            90f
        );
    }

    void PlaceFaces(GameObject givenGameObject)
    {
        MeshRenderer givenMeshRenderer = givenGameObject.GetComponent<MeshRenderer>();


        float emphasis = 0.0001f;
        // Top
        facePool[0].transform.position = new Vector3(
            givenMeshRenderer.bounds.center.x,
            givenMeshRenderer.bounds.max.y + emphasis,
            givenMeshRenderer.bounds.center.z
        );
        facePool[0].transform.localScale = new Vector3(
            givenMeshRenderer.bounds.size.x,
            1f,
            givenMeshRenderer.bounds.size.z
        ) * 0.1f;

        // North
        facePool[1].transform.position = new Vector3(
            givenMeshRenderer.bounds.center.x,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.max.z + emphasis
        );
        facePool[1].transform.localScale = new Vector3(
            givenMeshRenderer.bounds.size.x,
            1f,
            givenMeshRenderer.bounds.size.y
        ) * 0.1f;

        // South
        facePool[2].transform.position = new Vector3(
            givenMeshRenderer.bounds.center.x,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.min.z - emphasis
        );
        facePool[2].transform.localScale = new Vector3(
            givenMeshRenderer.bounds.size.x,
            1f,
            givenMeshRenderer.bounds.size.y
        ) * 0.1f;

        // East
        facePool[3].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x + emphasis,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.center.z
        );
        facePool[3].transform.localScale = new Vector3(
            givenMeshRenderer.bounds.size.y,
            1f,
            givenMeshRenderer.bounds.size.z
        ) * 0.1f;

        // West
        facePool[4].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x - emphasis,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.center.z
        );
        facePool[4].transform.localScale = new Vector3(
            givenMeshRenderer.bounds.size.y,
            1f,
            givenMeshRenderer.bounds.size.z
        ) * 0.1f;

    }

    /*
    void CreateCubes()
    {
        // Create and format an instance of a cube
        GameObject cornerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cornerCube.GetComponent<BoxCollider>().isTrigger = true;
        cornerCube.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        cornerCube.GetComponent<MeshRenderer>().receiveShadows = false;

        float cornerCubeSideLength = 0.20f;
        cornerCube.transform.localScale = new Vector3(
            cornerCubeSideLength,
            cornerCubeSideLength,
            cornerCubeSideLength
        );
        cornerCube.AddComponent<ResizeObject>();

        // Generate a cube pool
        cubePool = new GameObject[8];
        for (int i = 0; i < cubePool.Length; i++)
        {
            cubePool[i] = Instantiate(cornerCube);
            cubePool[i].SetActive(false);
        }

        // Destroy cube instance
        Destroy(cornerCube);
    }
    void PlaceCorners(MeshRenderer givenMeshRenderer)
    {
        // 1. minX, minY, minZ
        cubePool[0].transform.position = givenMeshRenderer.bounds.min;

        // 2. minX, minY, maxZ
        cubePool[1].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x,
            givenMeshRenderer.bounds.min.y,
            givenMeshRenderer.bounds.max.z
        );

        // 3. minX, maxY, maxZ
        cubePool[2].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x,
            givenMeshRenderer.bounds.max.y,
            givenMeshRenderer.bounds.max.z
        );

        // 4. maxX, maxY, maxZ
        cubePool[3].transform.position = givenMeshRenderer.bounds.max;

        // 5. minX, maxY, minZ
        cubePool[4].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x,
            givenMeshRenderer.bounds.max.y,
            givenMeshRenderer.bounds.min.z
        );

        // 6. maxX, maxY, minZ
        cubePool[5].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x,
            givenMeshRenderer.bounds.max.y,
            givenMeshRenderer.bounds.min.z
        );

        // 7. maxX, minY, minZ
        cubePool[6].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x,
            givenMeshRenderer.bounds.min.y,
            givenMeshRenderer.bounds.min.z
        );

        // 8. maxX, minY, maxZ
        cubePool[7].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x,
            givenMeshRenderer.bounds.min.y,
            givenMeshRenderer.bounds.max.z
        );
    }
    */

    void SetParent(GameObject parent)
    {
        for (int i = 0; i < facePool.Length; i++)
        {
            facePool[i].transform.SetParent(parent.transform);
        }
        SetEdgeParent(parent);
    }

    void SetEdgeParent(GameObject parent)
    {
        for (int i = 0; i < edgePool.Length; i++)
        {
            edgePool[i].transform.SetParent(parent.transform);
        }
    }

    void RemoveParent()
    {
        if (facePool[0].transform.parent != null)
        {
            facePool[0].transform.parent.transform.DetachChildren();
        }
    }

    void ActivateFacePool()
    {
        for (int i = 0; i < facePool.Length; i++)
        {
            facePool[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < edgePool.Length; i++)
        {
            edgePool[i].gameObject.SetActive(true);
        }
    }

    void DeactivateFacePool()
    {
        for (int i = 0; i < facePool.Length; i++)
        {
            facePool[i].gameObject.SetActive(false);
        }
    }
    void DeactivateEdgePool()
    {
        for (int i = 0; i < edgePool.Length; i++)
        {
            edgePool[i].gameObject.SetActive(false);
        }
    }


    void CreateEdgeLines()
    {
        // Create and format an instance of an edge
        GameObject edgeInstance = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        edgeInstance.GetComponent<CapsuleCollider>().enabled = false;
        edgeInstance.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        edgeInstance.GetComponent<MeshRenderer>().material.color = Color.black;

        // Generate an edge pool
        edgePool = new GameObject[12];
        for (int i = 0; i < edgePool.Length; i++)
        {
            edgePool[i] = Instantiate(edgeInstance);
            edgePool[i].SetActive(false);
        }

        // topNorth
        edgePool[0].name = "topNorth";
        // topSouth
        edgePool[1].name = "topSouth";
        // topEast
        edgePool[2].name = "topEast";
        // topWest
        edgePool[3].name = "topWest";

        // northEast
        edgePool[4].name = "northEast";
        // southEast
        edgePool[5].name = "southEast";
        // northWest
        edgePool[6].name = "northWest";
        // southWest
        edgePool[7].name = "southWest";

        // bottemNorth
        edgePool[8].name = "bottemNorth";
        // bottemSouth
        edgePool[9].name = "bottemSouth";
        // bottemEast
        edgePool[10].name = "bottemEast";
        // bottemWest
        edgePool[11].name = "bottemWest";

        // Destroy cube instance
        Destroy(edgeInstance);
    }

    private void ResetEdgeRotations()
    {
        // topNorth
        edgePool[0].transform.rotation = Quaternion.Euler(
             0f,
             0f,
             90f
         );
        // topSouth
        edgePool[1].transform.rotation = Quaternion.Euler(
             0f,
             0f,
             90f
         );
        // topEast
        edgePool[2].transform.rotation = Quaternion.Euler(
             90f,
             0f,
             0f
         );
        // topWest
        edgePool[3].transform.rotation = Quaternion.Euler(
             90f,
             0f,
             0f
         );

        // northEast
        edgePool[4].transform.rotation = Quaternion.Euler(Vector3.zero);
        // southEast
        edgePool[5].transform.rotation = Quaternion.Euler(Vector3.zero);
        // northWest
        edgePool[6].transform.rotation = Quaternion.Euler(Vector3.zero);
        // southWest
        edgePool[7].transform.rotation = Quaternion.Euler(Vector3.zero);

        // bottemNorth
        edgePool[8].transform.rotation = Quaternion.Euler(
             0f,
             0f,
             90f
         );
        // bottemSouth
        edgePool[9].transform.rotation = Quaternion.Euler(
             0f,
             0f,
             90f
         );
        // bottemEast
        edgePool[10].transform.rotation = Quaternion.Euler(
             90f,
             0f,
             0f
         );
        // bottemWest
        edgePool[11].transform.rotation = Quaternion.Euler(
             90f,
             0f,
             0f
         );
    }

    void PlaceEdges(GameObject givenGameObject)
    {
        MeshRenderer givenMeshRenderer = givenGameObject.GetComponent<MeshRenderer>();
        float radius = 0.1f;

        // topNorth
        edgePool[0].transform.position = new Vector3(
            givenMeshRenderer.bounds.center.x,
            givenMeshRenderer.bounds.max.y,
            givenMeshRenderer.bounds.max.z
        );
        edgePool[0].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.x * 0.5f,
            radius
        );

        // topSouth
        edgePool[1].transform.position = new Vector3(
            givenMeshRenderer.bounds.center.x,
            givenMeshRenderer.bounds.max.y,
            givenMeshRenderer.bounds.min.z
        );
        edgePool[1].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.x * 0.5f,
            radius
        );

        // topEast
        edgePool[2].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x,
            givenMeshRenderer.bounds.max.y,
            givenMeshRenderer.bounds.center.z
        );
        edgePool[2].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.z * 0.5f,
            radius
        );

        // topWest
        edgePool[3].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x,
            givenMeshRenderer.bounds.max.y,
            givenMeshRenderer.bounds.center.z
        );
        edgePool[3].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.z * 0.5f,
            radius
        );



        // northEast
        edgePool[4].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.max.z
        );
        edgePool[4].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.y * 0.5f,
            radius
        );

        // southEast
        edgePool[5].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.min.z
        );
        edgePool[5].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.y * 0.5f,
            radius
        );

        // northWest
        edgePool[6].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.max.z
        );
        edgePool[6].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.y * 0.5f,
            radius
        );

        // southWest
        edgePool[7].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x,
            givenMeshRenderer.bounds.center.y,
            givenMeshRenderer.bounds.min.z
        );
        edgePool[7].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.y * 0.5f,
            radius
        );



        // bottemNorth
        edgePool[8].transform.position = new Vector3(
            givenMeshRenderer.bounds.center.x,
            givenMeshRenderer.bounds.min.y,
            givenMeshRenderer.bounds.max.z
        );
        edgePool[8].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.x * 0.5f,
            radius
        );

        // bottemSouth
        edgePool[9].transform.position = new Vector3(
            givenMeshRenderer.bounds.center.x,
            givenMeshRenderer.bounds.min.y,
            givenMeshRenderer.bounds.min.z
        );
        edgePool[9].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.x * 0.5f,
            radius
        );

        // bottemEast
        edgePool[10].transform.position = new Vector3(
            givenMeshRenderer.bounds.max.x,
            givenMeshRenderer.bounds.min.y,
            givenMeshRenderer.bounds.center.z
        );
        edgePool[10].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.z * 0.5f,
            radius
        );

        // bottemWest
        edgePool[11].transform.position = new Vector3(
            givenMeshRenderer.bounds.min.x,
            givenMeshRenderer.bounds.min.y,
            givenMeshRenderer.bounds.center.z
        );
        edgePool[11].transform.localScale = new Vector3(
            radius,
            givenMeshRenderer.bounds.size.z * 0.5f,
            radius
        );
    }

    void RemoveEdgeParent()
    {
        for (int i = 0; i < edgePool.Length; i++)
        {
            edgePool[i].transform.parent = null;
        }
    }

    void ResizeEdges()
    {
        GameObject parent = edgePool[0].transform.parent.gameObject;
        RemoveEdgeParent();
        ResetEdgeRotations();
        PlaceEdges(parent);
        SetEdgeParent(parent);
    }
}
