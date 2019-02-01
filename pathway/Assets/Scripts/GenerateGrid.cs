using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateGrid : MonoBehaviour {

    public Material evenColor;

    public Material oddColor;

    public Material insideBorderMaterial;

    public Material outsideBorderMaterial;

    public PhysicMaterial wallPhysicMaterial;

    private int numOfRows;

    private int numOfCols;

    public void MakeGrid()
    {
        DontDestroyOnLoad(this.gameObject);
        numOfRows = GameController.gameController.numOfRows;
        numOfCols = GameController.gameController.numOfCols;
        PlaceGrid();
        PlaceBorders();
    }

    void PlaceGrid()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshRenderer floorMesh = floor.GetComponent<MeshRenderer>();
        floor.name = "Plot";
        floor.tag = "Floor";

        for (int row = 0; row < numOfRows; row++)
        {
            for (int col = 0; col < numOfCols; col++)
            {
                floorMesh.material = (((row % 2) + col) % 2 == 0 ? evenColor : oddColor);
                GameObject.Instantiate(floor, transform.position + new Vector3((float)col, 0f, (float)-row), floor.transform.rotation, transform);
            }
        }
        Destroy(floor);
    }

    void PlaceBorders()
    {
        float height = 2f;
        float width = 1f;
        float length = numOfRows + 2f;
        float farRow = (float)(numOfRows / 2f) - 0.5f;
        float nearRow = farRow;
        float farCol = (float)numOfCols;
        float nearCol = -1f;
        for (int i = 0; i < 2; i++)
        {
            if (i != 0) 
            {
                length = width * 2f;
                width = numOfCols + 2f;
                farRow = (float)numOfRows;
                nearRow = -1f;
                farCol = (float)(numOfCols / 2f) - 0.5f;
                nearCol = farCol;

            }


            GameObject insideBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            insideBorder.transform.localScale = new Vector3(width, height, length);
            insideBorder.GetComponent<MeshRenderer>().material = insideBorderMaterial;
            insideBorder.GetComponent<BoxCollider>().material = wallPhysicMaterial;

            Instantiate(insideBorder, transform.position + new Vector3(farCol, 1.5f,-farRow), insideBorder.transform.rotation, transform);
            Instantiate(insideBorder, transform.position + new Vector3(nearCol, 1.5f, -nearRow), insideBorder.transform.rotation, transform);
            Destroy(insideBorder);


            if (i != 0)
            {
                length /= 2f;
                width += 1f;
                farRow = ((float)numOfRows + length * 1.5f);
                nearRow = (-1f - length * 1.5f);

            }
            else
            {
                width /= 2f;
                length += 1f;
                farCol = ((float)numOfCols + width * 1.5f);
                nearCol = (-1f - width * 1.5f);
            }

            GameObject outsideBorder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outsideBorder.transform.localScale = new Vector3(width, height, length );
            outsideBorder.GetComponent<MeshRenderer>().material = outsideBorderMaterial;

            Instantiate(outsideBorder, transform.position + new Vector3(farCol, 1.5f, -farRow), outsideBorder.transform.rotation, transform);
            Instantiate(outsideBorder, transform.position + new Vector3(nearCol, 1.5f, -nearRow), outsideBorder.transform.rotation, transform);
            Destroy(outsideBorder);
        }

    }
}
