using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombinedStat
{
    [SerializeField]
    protected float baseValue = 0;
    public float BaseValue => baseValue;
    [SerializeField]
    protected float increaseValue = 0;
    public float IncreaseValue => increaseValue;
    [SerializeField]
    protected List<float> moreValues = new List<float>();
    public float MoreValue
    {
        get
        {
            float resultValue = 1;
            foreach (float value in moreValues)
            {
                resultValue *= 1 + (value / 100);
            }
            return resultValue;
        }
    }
    public List<float> MoreValues => moreValues;
    protected float value;
    public float Value => value;
    protected float modValue;
    public float ModValue => modValue;
    //{
    //    get
    //    {
    //        float resultValue = 1;
    //        foreach (float value in moreValue)
    //        {
    //            resultValue *= 1 + (value / 100);
    //        }
    //        resultValue *= 1 + (IncreaseValue / 100);
    //        return resultValue;
    //    }
    //}
    public CombinedStat()
    {
        recalculateValue();
    }

    public CombinedStat(float baseValue, float increaseValue, List<float> moreValue)
    {
        this.baseValue = baseValue;
        this.increaseValue = increaseValue;
        this.moreValues = moreValue;
        recalculateValue();
    }

    public virtual void SetValue(float baseValue, float increaseValue, List<float> moreValue)
    {
        this.baseValue = baseValue;
        this.increaseValue = increaseValue;
        this.moreValues.Clear();
        foreach (float more in moreValue)
            this.moreValues.Add(more);
        recalculateValue();
    }
    public void InitValue()
    {
        recalculateValue();
    }

    public virtual void AddBaseValue(float value)
    {
        baseValue += value;
        recalculateValue();
    }

    public void AddIncreaseValue(float value)
    {
        increaseValue += value;
        recalculateValue();
    }

    public void AddMoreValue(float value)
    {
        if (value >= 0)
            moreValues.Add(value);
        else
            moreValues.Add((100 - value) / 100);
        recalculateValue();
    }

    public void AddMoreValue(List<float> values)
    {
        foreach (float value in values)
        {
            if (value >= 0)
                moreValues.Add(value);
            else
                moreValues.Add((100 - value) / 100);
        }
        recalculateValue();
    }

    public virtual void RemoveBaseValue(float value)
    {
        baseValue -= value;
        recalculateValue();
    }

    public void RemoveIncreaseValue(float value)
    {
        increaseValue -= value;
        recalculateValue();
    }

    public void RemoveMoreValue(float value)
    {
        if (value < 0)
            value = -(100 - (value * 100));
        if (moreValues.Contains(value))
        {
            moreValues.Remove(value);
        }
        else
            Debug.LogWarning("Removing exception " + value);
        recalculateValue();
    }


    protected virtual void recalculateValue()
    {
        //value = baseValue * (1 + (increaseValue / 100)) * MoreValue;
        modValue = 1;
        foreach (float value in moreValues)
        {
            modValue *= 1 + (value / 100);
        }
        modValue *= 1 + (IncreaseValue / 100);
        value = baseValue * ModValue;
    }

    public virtual float ValueWithAddedParams(float baseValue, float increaseValue, List<float> moreValues) //не учитывает отрицательные значения
    {
        //float newBaseValue = 1;
        float newModValue = 1;
        foreach (float value in this.moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        foreach (float value in moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        newModValue *= 1 + ((IncreaseValue + increaseValue) / 100);
        return (this.baseValue + baseValue) * newModValue;
    }

    public virtual float ValueWithAddedParams(float baseValue) //не учитывает отрицательные значения
    {
        float newModValue = 1;
        foreach (float value in this.moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        newModValue *= 1 + (IncreaseValue / 100);
        return (this.baseValue + baseValue) * newModValue;
    }

    public virtual float ValueWithAddedParams(CombinedStat stat) //не учитывает отрицательные значения
    {
        float newModValue = 1;
        foreach (float value in stat.MoreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        foreach (float value in moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        newModValue *= 1 + ((IncreaseValue + stat.IncreaseValue) / 100);
        return (baseValue + stat.BaseValue) * newModValue;
    }
    //
    public virtual float ModValueWithAddedParams(float baseValue, float increaseValue, List<float> moreValues) //не учитывает отрицательные значения
    {
        float newModValue = 1;
        foreach (float value in this.moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        foreach (float value in moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        newModValue *= 1 + ((IncreaseValue + increaseValue) / 100);
        return 1 * newModValue;
    }

    public virtual float ModValueWithAddedParams(CombinedStat stat) //не учитывает отрицательные значения
    {
        float newModValue = 1;
        foreach (float value in stat.MoreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        foreach (float value in moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        newModValue *= 1 + ((IncreaseValue + stat.IncreaseValue) / 100);
        return 1 * newModValue;
    }
}
