using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;


public class PlayerLevelUpObserver : MonoBehaviour
{
    //public UnityEvent OnLevelUp = new UnityEvent();
    public GameObject LevelUpButton;
    private UnitActions player;

    public void Init(UnitActions player)
    {
        this.player = player;
        player.OnLevelUp.AddListener(PlayerLevelUp);
    }

    public void PlayerLevelUp(int level)
    {
        GlobalData.instance.PlayerData.LevelUp();
        LevelUpButton.SetActive(true);
        //OnLevelUp.Invoke();
    }
}
