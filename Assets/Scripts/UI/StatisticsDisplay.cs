using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatisticsDisplay : MonoBehaviour
{
    public TMP_Text MonsterLevel;

    private void Update() //Переписать как статистика будет готова
    {
        MonsterLevel.text = "Monster level: " + GlobalData.instance.LevelData.MonsterLevel;
    }
}
