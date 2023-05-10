using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public ScreenChange Screen;

    public void StartSelection()
    {
        Screen.SelectAreaScreen();
    }

    public void SelectArea(string areaName)
    {
        GlobalData.instance.LevelData.AreaName = areaName;
        Screen.SelectHeroScreen();
    }

    public void SelectHero(string heroName)
    {
        //GlobalData.instance.PlayerData.PlayerPrefabName = heroName;
        startLevel();
    }

    private void startLevel()
    {
        SceneManager.LoadScene(1);
    }
}
