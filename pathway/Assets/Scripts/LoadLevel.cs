using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

    private GameObject[] toolList;

    public GameObject spawnPlatformPrefab;
    public GameObject checkpointPrefab;
    public GameObject goalPrefab;


    void Start()
    {
        toolList = new GameObject[GameController.gameController.toolList.Length + GameController.gameController.constructToolsList.Length];
        GameController.gameController.toolList.CopyTo(toolList, 0);
        GameController.gameController.constructToolsList.CopyTo(toolList, GameController.gameController.toolList.Length);
    }

    public int GetLevelRows(string levelFileName)
    {
        int rows = GetLevelData(levelFileName).numOfRows;
        return rows;
    }

    public int GetLevelCols(string levelFileName)
    {
        int cols = GetLevelData(levelFileName).numOfCols;
        return cols;
    }

    private LevelDataClass GetLevelData(string levelFileName)
    {
        string levelFilePath = Path.Combine(EditLevelList.levelsFolderPath, levelFileName + ".json");
        if (File.Exists(levelFilePath))
        {
            string obstacleDataAsJson = File.ReadAllText(levelFilePath);
            Debug.Log(obstacleDataAsJson);
            LevelDataClass loadedData = JsonUtility.FromJson<LevelDataClass>(obstacleDataAsJson);
            return loadedData;
        }
        Debug.Log("No Previous Level Data");
        return null;
    }

    public void LoadLevelObstacles(string levelFileName)
    {
        Debug.Log("Loading Level");
        LevelDataClass loadedData = GetLevelData(levelFileName);
        if (loadedData != null)
        {
            GenerateObstaclesWithData(loadedData);
            GenerateSpawnPlatformWithData(loadedData);
            GenerateCheckpointsWithData(loadedData);
            GenerateGoalsWithData(loadedData);
        }
    }

    private GameObject FindObstacleInstance(ObstacleClass obstacleData)
    {
        GameObject tool = null;
        for (int i = 0; i < toolList.Length; i++)
        {
            if (toolList[i].name.Equals(obstacleData.typeOfObstacle))
            {
                tool = toolList[i];
                break;
            }
        }
        return tool;
    }

    private void GenerateObstaclesWithData(LevelDataClass levelData)
    {
        for (int i = 0; i < levelData.obstacles.Length; i++)
        {
            GameObject obstacleInstance = FindObstacleInstance(levelData.obstacles[i]);
            GenerateClone(levelData.obstacles[i], obstacleInstance);
        }
    }

    private void GenerateSpawnPlatformWithData(LevelDataClass levelData)
    {
        GameObject spawnPlatform = GenerateClone(levelData.spawnPlatform, spawnPlatformPrefab);
        GameController.gameController.SetSpawnLocation(spawnPlatform.transform.position);
    }

    private void GenerateCheckpointsWithData(LevelDataClass levelData)
    {
        for (int i = 0; i < levelData.checkpoints.Length; i++)
        {
            GenerateClone(levelData.checkpoints[i], checkpointPrefab);
        }
    }

    private void GenerateGoalsWithData(LevelDataClass levelData)
    {
        for (int i = 0; i < levelData.goals.Length; i++)
        {
            GenerateClone(levelData.goals[i], goalPrefab);
        }
    }

    private GameObject GenerateClone(ObstacleClass obstacleData, GameObject instanceOfObstacle)
    {
        GameObject obstacle;
        if (instanceOfObstacle != null)
        {

            Vector3 position = new Vector3(
                obstacleData.position[0],
                obstacleData.position[1],
                obstacleData.position[2]
            );

            Quaternion rotation = new Quaternion(
                obstacleData.rotation[0],
                obstacleData.rotation[1],
                obstacleData.rotation[2],
                obstacleData.rotation[3]
            );

            Vector3 scale = new Vector3(
                obstacleData.scale[0],
                obstacleData.scale[1],
                obstacleData.scale[2]
            );

            obstacle = Instantiate(instanceOfObstacle, position, rotation);

            DontDestroyOnLoad(obstacle);

            obstacle.transform.localScale = scale;

            obstacle.tag = obstacleData.tag;
            obstacle.GetComponent<MeshRenderer>().enabled = true;
            return obstacle;
        }
        else
        {
            return null;
        }
    }
}
