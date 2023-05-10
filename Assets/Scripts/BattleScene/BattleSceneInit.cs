using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneInit : MonoBehaviour
{
    public PlayerInit PlayerSetup;
    public LevelSetup AreaSetup;
    public CameraFollow Follower;
    public LevelStatistics Statistics;
    private EnemySpawn spawnSystem;
    private GameObject sceneData;

    void Start()
    {
        #region Player init
        CharacterList characterType = GlobalData.instance.PlayerData.PlayerCharacter;
        GameObject playerGameObject = PlayerSetup.CreatePlayerPrefab(characterType);
        Follower.SetTarget(playerGameObject.transform);
        #endregion

        //Arena generation
        AreaSetup.SetupLevel();

        //Adding scenario and his services
        sceneData = new GameObject();
        SurvivalScenario scenario = sceneData.AddComponent<SurvivalScenario>();
        AreaSetup.Spawner.CreatePool(); // �������� ���� � �������
        spawnSystem = sceneData.AddComponent<EnemySpawn>(); // ���������� ������� ������
        spawnSystem.Init(AreaSetup.Spawner, playerGameObject.transform);
        scenario.InitScenario(GlobalData.instance.LevelData.Map, spawnSystem, GlobalData.instance.PlayerData.Player.transform);

        //Start scenario
        GlobalData.instance.LevelData.CreateNewLevelData(1); //����������
        scenario.StartScenario(1); //��������� �������� � ��������� ������ 1

        //Timer set
        Statistics.SetTimer(true);
    }

}
