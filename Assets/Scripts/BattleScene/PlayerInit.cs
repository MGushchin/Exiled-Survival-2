using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInit : MonoBehaviour
{
    public PlayerInputControl InputControl;
    public PlayerLevelUpObserver LevelUpObserver;
    public EndOfTheGameExecute EndGameExecute;
    public GameObject WarriorPlayerPrefab;
    public Sliderbar Healthbar;
    public BackgroundSliderbar BackgroundSlider;
    public Sliderbar Experiencebar;
    public PlayerLevelUp LevelUp;
    public PlayerSkillsGiver SkillsGiver;
    public PlayerSkillsDisplay SkillsDisplay;
    public DynamicSkillsDisplayPanel DynamicSkillPanel;
    public BattleSceneMenuPanel Menu;
    //public UnitPool CurrentPool;

    public GameObject CreatePlayerPrefab(CharacterList prefabName)
    {
        GameObject targetPrefab = WarriorPlayerPrefab;
        GameObject player = Instantiate(targetPrefab);
        UnitActions playerActions = player.GetComponent<UnitActions>();
        //�������������� ������� �����
        playerActions.InitUnit();
        //����������� ����������
        InputControl.OnMovementClick.AddListener(playerActions.Move);
        InputControl.OnLookDirectionChange.AddListener(playerActions.LookInDirection);
        InputControl.OnSkillClick.AddListener(playerActions.UseSkill);
        InputControl.OnAutocastChange.AddListener(playerActions.SkillsActivation.SetAutocast);
        InputControl.OnOpenMenu.AddListener(Menu.SetMenu);
        //��������� ������
        LevelUp.Init(playerActions);
        //���������e ������
        SkillsDisplay.Init(playerActions.SkillsActivation);
        //������ ��������� ������
        SkillsGiver.Init(playerActions);
        //������������� �������� (�������) ������
        playerActions.SkillsActivation.Storage.Passives.InitSKill(playerActions);
        //����������� ������ � ����� �����
        GlobalData.instance.PlayerData.Player = player;
        UnitPool.instance.AddToPool(playerActions, true);
        LevelUpObserver.Init(playerActions);
        playerActions.Experience.SetLevel(1);
        playerActions.LifeValue.OnValueChanged.AddListener(Healthbar.SetValues);
        playerActions.LifeValue.OnValueChanged.AddListener(BackgroundSlider.SetValues);
        playerActions.OnExperienceChanged.AddListener(Experiencebar.SetValues);
        playerActions.OnDeath.AddListener(EndGameExecute.RenderGameOverPanel);
        DynamicSkillPanel.SetPlayer(playerActions);
        return player;
    }
}
