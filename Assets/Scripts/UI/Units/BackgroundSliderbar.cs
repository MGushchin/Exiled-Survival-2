using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSliderbar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private float speed = 1;
    private float targetValue;
    private bool coroutineStarted = false;
    private IEnumerator changeCurrentValueCoroutine;
    private bool active = false;

    public void SetValues(float value, float maxValue)
    {
        slider.maxValue = maxValue;
        targetValue = value;
        if (!coroutineStarted && targetValue < slider.value && active)
        {
            changeCurrentValueCoroutine = changeCurrentValue();
            StartCoroutine(changeCurrentValueCoroutine);
        }
        else
            slider.value = value;
    }

    private IEnumerator changeCurrentValue()
    {
        coroutineStarted = true;
        while(targetValue < slider.value)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        coroutineStarted = false;
    }

    private void OnDisable()
    {
        active = false;
        if (coroutineStarted)
        {
            coroutineStarted = false;
            StopCoroutine(changeCurrentValueCoroutine);
    }

}
    private void OnEnable()
    {
        active = true;
    }
}
