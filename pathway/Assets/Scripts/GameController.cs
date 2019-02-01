using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject[] constructToolsList;
    public GameObject[] toolList;

    public enum BallStatus { Rest, Charging, Moving };
    public enum ViewMode { Tracking, Spectate };
    public enum LevelMode { Playing, Constructing };

    public BallStatus ballStatus;
    public ViewMode viewMode;
    public LevelMode levelMode;
    
    public static GameController gameController;

    public static Vector3 playerSpawnLocation;

    public GameObject playerPrefab;

    public GameObject player;
    
    public float levelGravity;

    public bool buildMode;
    public bool developerMode; // DEV MODE
    public GameObject focusObject;

    public string levelName;
    public int numOfRows;
    public int numOfCols;

    private int minimumRows = 5;
    private int mimimumCols = 5;

    private void Awake()
    {
        if (gameController == null)
        {
            DontDestroyOnLoad(gameObject);
            gameController = this;
        }
        else if (gameController != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
        levelName = null;
        numOfCols = 0;
        numOfRows = 0;

        buildMode = false;
        developerMode = false; // DEV MODE
        SceneManager.sceneLoaded += GetComponent<OutlineBox>().OnSceneLoaded;
    }

    public void ToggleSpectateMode()
    {
        gameController.viewMode = (gameController.viewMode == ViewMode.Spectate ? ViewMode.Tracking : ViewMode.Spectate);
    }

    public void ActivateChargeMode()
    {
        gameController.ballStatus = BallStatus.Charging;
    }

    public void ReleaseChargeMode()
    {
        gameController.ballStatus = BallStatus.Moving;
    }

    public void SetSpawnLocation(Vector3 location)
    {
        playerSpawnLocation = new Vector3(
            location.x,
            location.y + playerPrefab.transform.localScale.y/2,
            location.z
        );
    }

    public void SpawnPlayer()
    {
        gameController.player = Instantiate(playerPrefab, playerSpawnLocation, playerPrefab.transform.rotation);
    }

    public void RespawnPlayer()
    {
        player.GetComponent<Rigidbody>().Sleep();
        player.transform.position = playerSpawnLocation;
    }

    public void UpdateLevelName(string givenName)
    {
        GameController.gameController.levelName = givenName;
    }

    public void UpdateRows(string rowsAsString)
    {
        GameController.gameController.numOfRows = int.Parse(rowsAsString);
    }

    public void UpdateColumns(string colsAsString)
    {
        GameController.gameController.numOfCols = int.Parse(colsAsString);
    }

    public void CreateLevel()
    {
        developerMode = true;
        if (GameController.gameController.numOfRows >= minimumRows && GameController.gameController.numOfCols >= mimimumCols && GameController.gameController.levelName.Length > 0 && !EditLevelList.IsAlreadyLevel(GameController.gameController.levelName))
        {
            GameController.gameController.levelMode = LevelMode.Constructing;
            LoadLevel();
        }
        else //Leave debug for now
        {
            if (EditLevelList.IsAlreadyLevel(levelName))
            {
                Debug.Log(levelName + " has already been used!");
            }
        }
    }

    public void PlayLevel()
    {
        if (developerMode)
        {
            GameController.gameController.levelMode = LevelMode.Constructing;
        }
        else
        {
            GameController.gameController.levelMode = LevelMode.Playing;
        }
        LoadLevel();
    }

    // Called by CreateLevel(), EditLevel(), and PlayLevel()
    private void LoadLevel()
    {
        SceneManager.LoadScene("GenerateGrid");
    }

    public void MakeGrid()
    {
        GameObject.FindGameObjectWithTag("Grid").GetComponent<GenerateGrid>().MakeGrid();
    }

    public void LoadLevelObstacles()
    {
        gameObject.GetComponent<LoadLevel>().LoadLevelObstacles(levelName);
    }

    public void SaveLevel()
    {
        gameObject.GetComponent<SaveLevel>().SaveLevelData();
    }

    public void SetLevel(string levelFileName)
    {
        GameController.gameController.levelName = levelFileName;
        LoadLevel loadLevel = gameObject.GetComponent<LoadLevel>();
        GameController.gameController.numOfRows = loadLevel.GetLevelRows(levelFileName);
        GameController.gameController.numOfCols = loadLevel.GetLevelCols(levelFileName);
    }

    public void ReturnToMenu()
    {
        GameObject[] gameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].active)
            {
                Destroy(gameObjects[i]);
            }
        }
        SceneManager.LoadScene("MainMenu");
    }

    public GameObject SelectObject()
    {
        RaycastHit hit;
        Ray thisRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(thisRay, out hit))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    public bool IsObjectPlaceable(GameObject givenGameObject)
    {
        MeshRenderer meshRenderer = givenGameObject.GetComponent<MeshRenderer>();
        // Inside the grid
        if (meshRenderer.bounds.min.x <= -1f || meshRenderer.bounds.max.z >= 1f ||
            meshRenderer.bounds.max.x > GameController.gameController.numOfCols || meshRenderer.bounds.min.z < -GameController.gameController.numOfRows ||
            ObjectInsideBesideFloorOrSelf(givenGameObject))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    bool ObjectInsideBesideFloorOrSelf(GameObject givenGameObject)
    {
        Collider[] inside = Physics.OverlapBox(
            givenGameObject.transform.position,
            givenGameObject.GetComponent<MeshRenderer>().bounds.size / 2.1f
        );
        for (int i = 0; i < inside.Length; i++)
        {
            if (inside[i].tag != "Floor" && inside[i].gameObject != givenGameObject && (inside[i].transform.parent == null || inside[i].transform.parent.gameObject != givenGameObject))
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject clicked = SelectObject();
            if (clicked != null)
            {
                if ((clicked.scene == SceneManager.GetActiveScene() && clicked.tag == "Obstacle") ||
                    (GameController.gameController.developerMode && (clicked.tag == "Obstacle" || IsToolInConstructToolsList(clicked))))
                {
                    this.gameObject.GetComponent<OutlineBox>().ShowOutline(clicked);
                }
                else if (clicked.transform.parent == null || !(clicked.transform.parent.tag == "Obstacle" || IsToolInConstructToolsList(clicked.transform.parent.gameObject)))
                {
                    this.gameObject.GetComponent<OutlineBox>().RemoveOutline();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && !buildMode)
        {
            this.gameObject.GetComponent<OutlineBox>().RemoveOutline();
        }
    }

    bool IsToolInConstructToolsList(GameObject givenGameObject)
    {
        for (int i = 0; i < constructToolsList.Length; i++)
        {
            if (givenGameObject.tag == constructToolsList[i].tag)
            {
                return true;
            }
        }
        return false;
    }
}
