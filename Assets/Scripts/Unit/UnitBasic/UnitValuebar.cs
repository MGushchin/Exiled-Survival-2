using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitValuebar : MonoBehaviour
{
    [SerializeField]
    private string valueName;
    public string ValueName => valueName;
    public UnityEvent<float, float> OnValueChanged = new UnityEvent<float, float>();
    public UnityEvent<float> OnReachingFull = new UnityEvent<float>();
    public UnityEvent OnReachingZero = new UnityEvent();
    private float value;
    private float maxValue;

    public void InitValue(string newName, float value, float maxValue)
    {
        valueName = newName;
        this.value = value;
        this.maxValue = maxValue;
    }

    public void InitValue(float value, float maxValue)
    {
        this.value = value;
        this.maxValue = maxValue;
    }

    public void AddValue(float addedValue)
    {
        value = Mathf.Clamp(value + addedValue, 0, maxValue);
        OnValueChanged.Invoke(value, maxValue);
    }
}
