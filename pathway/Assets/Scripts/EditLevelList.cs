using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class EditLevelList
{
    public static string levelListFilePath = Path.Combine(Application.streamingAssetsPath, "levelList.json");

    public static string levelsFolderPath = Path.Combine(Application.streamingAssetsPath, "levelsFolder");


    public static void AddLevel(string levelName)
    {
        LevelListClass levelListData = GetLevelListData();
        levelListData.levelNames.Add(levelName);
        SaveLevelListData(levelListData);
    }

    public static void RemoveLevel(string levelName)
    {
        LevelListClass levelListData = GetLevelListData();
        levelListData.levelNames.Remove(levelName);
        SaveLevelListData(levelListData);
    }

    public static void UpdateLevelName(string oldLevelName, string newLevelName)
    {
        LevelListClass levelListData = GetLevelListData();
        int oldNameIndex = 0;
        while (!levelListData.levelNames[oldNameIndex].Equals(oldLevelName))
        {
            oldNameIndex++;
        }
        levelListData.levelNames[oldNameIndex] = newLevelName;
    }

    public static void DeleteLevel(string levelName)
    {
        RemoveLevel(levelName);
        string levelPath = Path.Combine(levelsFolderPath, levelName);
        File.Delete(levelPath);
    }

    public static bool IsAlreadyLevel(string levelName)
    {
        List<string> levelNames = GetLevelNames();
        return levelNames.Contains(levelName);
    }

    public static List<string> GetLevelNames()
    {
        LevelListClass levelListData = GetLevelListData();
        return levelListData.levelNames;
    }

    public static string GetLevelNameAt(int index)
    {
        LevelListClass levelDataClass = GetLevelListData();
        return levelDataClass.levelNames[index];
    }

    public static int GetLevelCount()
    {
        LevelListClass levelListData = GetLevelListData();
        int count = levelListData.levelNames.Count;
        return count;
    }

    private static LevelListClass GetLevelListData()
    {
        if (!File.Exists(levelListFilePath))
        {
            LevelListClass emptyLevelList = new LevelListClass();
            emptyLevelList.levelNames = new List<string>();
            SaveLevelListData(emptyLevelList);
        }
        string levelListDataAsJson = File.ReadAllText(levelListFilePath);
        LevelListClass levelListData = JsonUtility.FromJson<LevelListClass>(levelListDataAsJson);
        return levelListData;
    }

    private static void SaveLevelListData(LevelListClass levelListData)
    {
        string levelListDataAsJson = JsonUtility.ToJson(levelListData);
        File.WriteAllText(levelListFilePath, levelListDataAsJson);
    }
}
