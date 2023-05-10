using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Valuebar : MonoBehaviour
{
    [SerializeField]
    private string valueName;
    public string Name => valueName;
    [HideInInspector]
    public UnityEvent OnReachingZero = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnReachingMaximum = new UnityEvent();
    public UnityEvent<float, float> OnValueChanged = new UnityEvent<float, float>();
    [SerializeField]
    private float currentValue;
    public float Value => currentValue;
    [SerializeField]
    private float maximumValue;
    public float MaximumValue => maximumValue;

    public void AddValue(float value)
    {
        currentValue = Mathf.Clamp(currentValue + value, 0, maximumValue);
        if (currentValue == 0)
            OnReachingZero.Invoke();
        if (currentValue == maximumValue)
            OnReachingMaximum.Invoke();
        OnValueChanged.Invoke(currentValue, maximumValue);
    }
    public void AddMaximumValue(float value)
    {
        float oldMaxValue = maximumValue;
        maximumValue = value;
        currentValue = currentValue * maximumValue / oldMaxValue;
        currentValue = Mathf.Clamp(currentValue, 0, maximumValue);
        OnValueChanged.Invoke(currentValue, maximumValue);
    }

    public void SetMaximumValue(float value)
    {
        maximumValue = value;
        currentValue = Mathf.Clamp(currentValue, 0, maximumValue);
        OnValueChanged.Invoke(currentValue, maximumValue);
    }

    public void FillToMaximum()
    {
        currentValue = maximumValue;
        OnValueChanged.Invoke(currentValue, maximumValue);
    }
}
