using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationLevelProgression : MonoBehaviour
{
    private int levelCountPerTick = 1;
    private float levelUpFrequency = 1;
    private IEnumerator levelUpCoroutine;
    private bool timerStarted = false;

    private void Start()
    {
        StatisticsDisplay.instance.RegisterData("AreaLevel", "Area level: " + GlobalData.instance.LevelData.MonsterLevel);
    }

    public void AddLevel(int levelCount)
    {
        Debug.Log("AddLevel");
        GlobalData.instance.LevelData.AddMonsterLevel(levelCount);
        StatisticsDisplay.instance.UpdateStat("AreaLevel", "Area level: " + GlobalData.instance.LevelData.MonsterLevel);
    }

    public void SetTimerSetting(int levelCount, float frequency)
    {
        levelCountPerTick = levelCount;
        levelUpFrequency = frequency;
    }

    public void StartTimer()
    {
        if (!timerStarted)
        {
            levelUpCoroutine = levelUpUpdate();
            StartCoroutine(levelUpCoroutine);
        }
    }

    public void StopTimer()
    {
        if (timerStarted)
        {
            StopCoroutine(levelUpCoroutine);
            timerStarted = false;
        }
    }

    private IEnumerator levelUpUpdate()
    {
        timerStarted = true;
        StatisticsDisplay.instance.UpdateStat("AreaLevel", "Area level: " + GlobalData.instance.LevelData.MonsterLevel);
        while (timerStarted)
        {
            yield return new WaitForSeconds(levelUpFrequency);
            AddLevel(levelCountPerTick);
        }
        timerStarted = false;
    }
}
