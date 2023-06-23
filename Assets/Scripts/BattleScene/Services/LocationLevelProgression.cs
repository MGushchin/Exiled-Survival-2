using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationLevelProgression : MonoBehaviour
{
    private int levelCountPerTick = 1;
    private float levelUpFrequency = 1;
    private IEnumerator levelUpCoroutine;
    private bool timerStarted = false;

    public void AddLevel(int levelCount)
    {
        GlobalData.instance.LevelData.AddMonsterLevel(levelCount);
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
        while(timerStarted)
        {
            yield return new WaitForSeconds(levelUpFrequency);
            AddLevel(levelCountPerTick);
        }
        timerStarted = false;
    }
}
