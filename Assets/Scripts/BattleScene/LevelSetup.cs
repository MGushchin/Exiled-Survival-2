using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public enum Areas
{
    Dungeon
}

public enum Scenario
{
    Survival
}

public class LevelSetup : MonoBehaviour
{
    public Tilemap WalkableTilemap;
    public Tilemap BlockingTilemap;
    public GameObject DungeonPrefab;
    public NavMeshSurface Navigation;
    public EnemyFactory Spawner;
    public LocationLevelProgression Progression;
    private SurvivalScenario scenario;
    private ILevelSetup currentLevel;

    public void SetupLevel()
    {
        CreateTerrain(Areas.Dungeon, new Vector2(50, 50));
    }

    public void CreateTerrain(Areas areaType, Vector2 size)
    {
        switch (areaType)
        {
            case (Areas.Dungeon):
                {
                    currentLevel = Instantiate(DungeonPrefab).GetComponent<DungeonSetup>();
                }
                break;
            default:
                {
                    currentLevel = Instantiate(DungeonPrefab).GetComponent<DungeonSetup>();
                    Debug.LogWarning("Default exception");
                }
                break;
        }
        currentLevel.InitSetup(WalkableTilemap, BlockingTilemap);
        currentLevel.selfObject.transform.position = new Vector3(-size.x / 2, -size.y / 2, 0);
        //GlobalData.instance.LevelData.Map = currentLevel.Setup((int)size.x, 1);
        //GlobalData.instance.LevelData.Map = currentLevel.SetupRandomly((int)size.x, (int)size.y);
        GlobalData.instance.LevelData.Map = currentLevel.Setup((int)size.x, (int)size.y);
        Navigation.BuildNavMesh();
        Progression.SetTimerSetting(1, 60);
        Progression.StartTimer();
    }
}
