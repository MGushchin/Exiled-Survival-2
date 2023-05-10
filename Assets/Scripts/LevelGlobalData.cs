using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LevelData
{
    public int MonsterLevel;
    public float IncreasedMobRarity;
    public float IncreasedMobQuantity;

    public LevelData(int level)
    {
        MonsterLevel = level;
        IncreasedMobRarity = 0;
        IncreasedMobQuantity = 0;
    }
}

public class LevelGlobalData : MonoBehaviour
{
    private bool levelCompleted = false;
    public bool LevelCompleted => levelCompleted;
    private LevelData data; //Поработать над инкапсуляцией
    public int MonsterLevel => data.MonsterLevel;
    public float IncreasedMobRarity => data.IncreasedMobRarity;
    public float IncreasedMobQuantity => data.IncreasedMobRarity;
    public string AreaName;
    public MapData Map;

    public void CreateNewLevelData(int level)
    {
        data = new LevelData(level);
    }

    public void UpdateLevelData(float addedQuantity, float addedRarity)
    {
        data.IncreasedMobQuantity += addedQuantity;
        data.IncreasedMobRarity += addedRarity;
    }

    public void AddMonsterLevel(int addedLevel)
    {
        data.MonsterLevel += addedLevel;
    }

    public void CompleteLevel()
    {
        levelCompleted = true;
    }
}
