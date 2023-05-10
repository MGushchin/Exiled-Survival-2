using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatistics : MonoBehaviour
{
    public static LevelStatistics instance;

    //Служебные
    private bool timerActive = false;
    private IEnumerator timerCoroutine;
    //Статистика
    private float seconds = 0;
    public float Seconds => seconds;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetTimer(bool value)
    {
        timerActive = value;
        timerCoroutine = timer();
        StartCoroutine(timerCoroutine);
    }

    private IEnumerator timer()
    {
        while (timerActive)
        {
            seconds += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
