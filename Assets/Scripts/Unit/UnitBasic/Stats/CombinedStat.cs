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
                //resultValue *= 1 + (value / 100);
                if (value >= 0)
                    resultValue *= 1 + (value / 100);
                else
                    resultValue *= ((100 - value) / 100);
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

    public void AddStat(CombinedStat stat)
    {
        baseValue += stat.baseValue;
        increaseValue += stat.increaseValue;
        foreach (float more in stat.moreValues)
        {
            //if (value >= 0)
                moreValues.Add(more);
            //else
            //    moreValues.Add((100 - more) / 100);
        }
        recalculateValue();
    }

    public void AddModValues(CombinedStat stat)
    {
        increaseValue += stat.increaseValue;
        foreach (float more in stat.moreValues)
            moreValues.Add(more);
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
        Debug.Log("more value " + value);
        //if (value >= 0)
            moreValues.Add(value);
        //else
        //    moreValues.Add((100 - value) / 100);
        recalculateValue();
    }

    public void AddMoreValue(List<float> values)
    {
        foreach (float value in values)
        {
            //if (value >= 0)
                moreValues.Add(value);
            //else
            //    moreValues.Add((100 - value) / 100);
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
        //if (value < 0)
        //    value = -(100 - (value * 100));
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
            //modValue *= 1 + (value / 100);

            if (value >= 0)
            {
                if (value == 0)
                    Debug.Log("value = 0 " + modValue);
                modValue *= 1 + (value / 100);
                Debug.Log("value = " + modValue);
            }
            else
                modValue *= ((100 + value) / 100);

        }

        float resultIncreaseValue = 1 + (IncreaseValue  / 100);

        if (resultIncreaseValue < 0)
            resultIncreaseValue = 0;

        modValue *= resultIncreaseValue;

        value = baseValue * ModValue;
    }

    public virtual float ValueWithAddedParams(float _baseValue, float _increaseValue, List<float> _moreValues)
    {
        //float newBaseValue = 1;
        float newModValue = 1;
        foreach (float value in this.moreValues)
        {
            //newModValue *= 1 + (value / 100);

            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 + value) / 100);
        }
        foreach (float value in _moreValues)
        {
            //newModValue *= 1 + (value / 100);

            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 + value) / 100);
        }

        float resultIncreaseValue = 1 + ((IncreaseValue + _increaseValue) / 100);

        if (resultIncreaseValue < 0)
            resultIncreaseValue = 0;

        newModValue *= resultIncreaseValue;

        return (this.baseValue + _baseValue) * newModValue;
    }

    public virtual float ValueWithAddedParams(float baseValue)
    {
        float newModValue = 1;
        foreach (float value in this.moreValues)
        {
            //newModValue *= 1 + (value / 100);

            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 + value) / 100);
        }

        float resultIncreaseValue = 1 + ((IncreaseValue) / 100);

        if (resultIncreaseValue < 0)
            resultIncreaseValue = 0;

        newModValue *= resultIncreaseValue;

        return (this.baseValue + baseValue) * newModValue;
    }

    public virtual float ValueWithAddedParams(CombinedStat stat) 
    {
        float newModValue = 1;
        foreach (float value in stat.MoreValues)
        {
            //newModValue *= 1 + (value / 100);

            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 + value) / 100);
        }
        foreach (float value in moreValues)
        {
            //newModValue *= 1 + (value / 100);
            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 + value) / 100);
        }

        float resultIncreaseValue = 1 + ((IncreaseValue + stat.increaseValue) / 100);

        if (resultIncreaseValue < 0)
            resultIncreaseValue = 0;

        newModValue *= resultIncreaseValue;

        return (baseValue + stat.BaseValue) * newModValue;
    }

    public virtual float ValueWithAddedModParams(CombinedStat stat)
    {
        float newModValue = 1;
        foreach (float value in stat.MoreValues)
        {
            //newModValue *= 1 + (value / 100);

            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 + value) / 100);
        }
        foreach (float value in moreValues)
        {
            //newModValue *= 1 + (value / 100);
            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 + value) / 100);
        }

        float resultIncreaseValue = 1 + ((IncreaseValue + stat.increaseValue) / 100);

        if (resultIncreaseValue < 0)
            resultIncreaseValue = 0;

        newModValue *= resultIncreaseValue;

        return baseValue * newModValue;
    }
    //
    public virtual float ModValueWithAddedParams(float _baseValue, float _increaseValue, List<float> _moreValues)
    {
        float newModValue = 1;
        foreach (float value in this.moreValues)
        {
            //newModValue *= 1 + (value / 100);
            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 - value) / 100);
        }
        foreach (float value in _moreValues)
        {
            //newModValue *= 1 + (value / 100);
            if (value >= 0)
                newModValue *= 1 + (value / 100);
            else
                newModValue *= ((100 - value) / 100);
        }

        float resultIncreaseValue = 1 + ((IncreaseValue + _increaseValue) / 100);

        if (resultIncreaseValue < 0)
            resultIncreaseValue = 0;

        newModValue *= resultIncreaseValue;

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

        float resultIncreaseValue = 1 + ((IncreaseValue + stat.IncreaseValue) / 100);

        if (resultIncreaseValue < 0)
            resultIncreaseValue = 0;

        newModValue *= resultIncreaseValue;

        return 1 * newModValue;
    }
}
