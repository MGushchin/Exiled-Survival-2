using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sliderbar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    public void SetValues(float value, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = value;
    }
}
