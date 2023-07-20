using System.Collections;
using System.Collections.Generic;
using UIMarkers;
using UnityEngine;

public class BattleSceneInit : MonoBehaviour
{
    public PlayerInit PlayerSetup;
    public LevelSetup AreaSetup;
    public CameraFollow Follower;
    public LevelStatistics Statistics;
    public Canvas MainCanvas;
    private EnemySpawn spawnSystem;
    private GameObject sceneData;

    void Start()
    {
        #region Player init
        CharacterList characterType = GlobalData.instance.PlayerData.PlayerCharacter;
        GameObject playerGameObject = PlayerSetup.CreatePlayerPrefab(characterType);
        Follower.SetTarget(playerGameObject.transform);
        #endregion

        //Marker system init
        MarkerSystem.instance.Init(playerGameObject.transform, MainCanvas.transform);

        //Map data
        GlobalData.instance.LevelData.CreateNewLevelData(1); //Переписать

        //Arena generation
        AreaSetup.SetupLevel();

        //Adding scenario and his services
        sceneData = new GameObject();
        sceneData.name = "Scenario holder";
        SurvivalScenario scenario = sceneData.AddComponent<SurvivalScenario>();
        AreaSetup.Spawner.CreatePool(); // Создание пула у фабрики
        spawnSystem = sceneData.AddComponent<EnemySpawn>(); // Добавление системы спавна
        spawnSystem.Init(AreaSetup.Spawner, playerGameObject.transform);
        scenario.InitScenario(GlobalData.instance.LevelData.Map, spawnSystem, GlobalData.instance.PlayerData.Player.transform);

        //Start scenario
        scenario.StartScenario(1); //Запускаем сценарий и запускаем стадию 1

        //Timer set
        //Statistics.SetTimer(true);
    }

}
