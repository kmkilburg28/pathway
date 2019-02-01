using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelDataClass
{
    public int numOfRows;
    public int numOfCols;
    public ObstacleClass[] obstacles;
    public ObstacleClass spawnPlatform;
    public ObstacleClass[] checkpoints;
    public ObstacleClass[] goals;
}
