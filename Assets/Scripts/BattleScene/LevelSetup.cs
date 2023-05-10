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
    private SurvivalScenario scenario;
    private ILevelSetup currentLevel;

    public void SetupLevel()
    {
        CreateTerrain(Areas.Dungeon, new Vector2(25, 25));
        //AddScenario(Scenario.Survival);
        //StartScenario();
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
        GlobalData.instance.LevelData.Map = currentLevel.Setup((int)size.x, 1);
        Navigation.BuildNavMesh();
    }

    //public void AddScenario(Scenario scenarioType)
    //{
    //    switch (scenarioType)
    //    {
    //        case (Scenario.Survival):
    //            {
    //                scenario = scenario = gameObject.AddComponent<SurvivalScenario>();
    //            }
    //            break;
    //        default:
    //            {
    //                scenario = scenario = gameObject.AddComponent<SurvivalScenario>();
    //                Debug.LogWarning("Default exception");
    //            }
    //            break;
    //    }
    //    scenario.InitScenario(GlobalData.instance.LevelData.Map, Spawner, GlobalData.instance.PlayerData.Player.transform);
    //}

    //public void StartScenario()
    //{
    //    GlobalData.instance.LevelData.CreateNewLevelData(1);
    //    scenario.StartScenario(1);
    //}

}
