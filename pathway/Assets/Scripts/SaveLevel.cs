using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLevel : MonoBehaviour {

    private LevelDataClass savedLevelData = new LevelDataClass();
    private int savedObstacleIndex;
    private int savedCheckpointIndex;
    private int savedGoalIndex;
    private string saveFileName;

    public void SaveLevelData()
    {

        saveFileName = GameController.gameController.levelName + ".json";

        savedLevelData.numOfRows = GameController.gameController.numOfRows;
        savedLevelData.numOfCols = GameController.gameController.numOfCols;
        Debug.Log("Saving obstacles...");
        // Collect information on all obstacles within the lvel
        GameObject[] obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
        savedLevelData.obstacles = new ObstacleClass[obstacleList.Length];
        savedObstacleIndex = 0;
        for (int i = 0; i < obstacleList.Length; i++)
        {
            AddObstacleData(obstacleList[i]);
        }
        Debug.Log("Saving spawn platform...");
        // Collect informatino about where the player is within the level
        GameObject spawnPlatform = GameObject.FindGameObjectWithTag("SpawnPlatform");
        if (spawnPlatform == null)
        {
            Debug.Log("Spawn platform is required in order to save!");
        }
        AddInitialSpawnData(spawnPlatform);

        Debug.Log("Saving checkpoints...");
        GameObject[] checkpointList = GameObject.FindGameObjectsWithTag("Checkpoint");
        savedLevelData.checkpoints = new ObstacleClass[checkpointList.Length];
        savedCheckpointIndex = 0;
        for (int i = 0; i < checkpointList.Length; i++)
        {
            AddCheckPointData(checkpointList[i]);
        }
        Debug.Log("Saving goals...");
        // Collect information on all goals within the level
        GameObject[] goalList = GameObject.FindGameObjectsWithTag("Goal");
        if (goalList.Length == 0)
        {
            Debug.Log("At least one goal is required in order to save!");
        }
        savedLevelData.goals = new ObstacleClass[goalList.Length];
        savedGoalIndex = 0;
        for (int i = 0; i < goalList.Length; i++)
        {
            AddGoalData(goalList[i]);
        }
        Debug.Log("DONE! Saving file");


        string obstacleDataAsJson = JsonUtility.ToJson(savedLevelData);

        string filePath = Path.Combine(EditLevelList.levelsFolderPath, saveFileName);
        File.WriteAllText(filePath, obstacleDataAsJson);

        // Successful save, so add to level list if not already there
        if (!EditLevelList.IsAlreadyLevel(GameController.gameController.levelName))
        {
            EditLevelList.AddLevel(GameController.gameController.levelName);
        }
    }

    private void AddObstacleData(GameObject obstacle)
    {

        savedLevelData.obstacles[savedObstacleIndex] = GenerateObstacleData(obstacle);
        savedObstacleIndex++;
    }

    private void AddInitialSpawnData(GameObject spawnPlatform)
    {
        savedLevelData.spawnPlatform = GenerateObstacleData(spawnPlatform);
    }

    private void AddCheckPointData(GameObject checkpoint)
    {
        savedLevelData.checkpoints[savedCheckpointIndex] = GenerateObstacleData(checkpoint);
        savedCheckpointIndex++;
    }

    private void AddGoalData(GameObject goal)
    {
        savedLevelData.goals[savedGoalIndex] = GenerateObstacleData(goal);
        savedGoalIndex++;
    }

    private ObstacleClass GenerateObstacleData(GameObject obstacle)
    {
        ObstacleClass retObstacle = new ObstacleClass();
        retObstacle.typeOfObstacle = obstacle.name.Split('(')[0];
        retObstacle.tag = obstacle.tag;

        retObstacle.position = new float[3] {
            obstacle.transform.position.x,
            obstacle.transform.position.y,
            obstacle.transform.position.z
         };

        retObstacle.rotation = new float[4] {
            obstacle.transform.rotation.x,
            obstacle.transform.rotation.y,
            obstacle.transform.rotation.z,
            obstacle.transform.rotation.w
         };

        retObstacle.scale = new float[3] {
            obstacle.transform.localScale.x,
            obstacle.transform.localScale.y,
            obstacle.transform.localScale.z
         };

        return retObstacle;
    }
}
