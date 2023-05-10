using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingDelay : MonoBehaviour //Под рефакторинг
{
    [SerializeField]
    private float delayTime = 1;
    private IEnumerator delayCoroutine;

    public void SetDelay(float newDelayTime)
    {
        delayTime = newDelayTime;
    }

    public void DisappearNow(Vector3 pos)//Переписать
    {
        StopCoroutine(delayCoroutine);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        delayCoroutine = delay(delayTime);
        StartCoroutine(delayCoroutine);
    }

    private IEnumerator delay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        gameObject.SetActive(false);
    }
}
