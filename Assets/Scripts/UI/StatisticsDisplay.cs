using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatisticsDisplay : MonoBehaviour
{
    public static StatisticsDisplay instance;
    public TMP_Text MonsterLevel;
    private Dictionary<string, string> statistics = new Dictionary<string, string>();

    private void Awake()
    {
        instance = this;
    }

    public void RegisterData(string key, string defaultValue)
    {
        if(!statistics.ContainsKey(key))
        {
            statistics.Add(key, defaultValue);
        }
        else
            Debug.Log("Multiply key registry");
    }

    public void UpdateStat(string key, string value)
    {
        statistics[key] = value;
    }

    private void Update() //Переписать как статистика будет готова
    {
        //MonsterLevel.text = "Monster level: " + GlobalData.instance.LevelData.MonsterLevel;
        MonsterLevel.text = "";
        foreach (string value in statistics.Values)
            MonsterLevel.text += value + "\n";
    }
}
