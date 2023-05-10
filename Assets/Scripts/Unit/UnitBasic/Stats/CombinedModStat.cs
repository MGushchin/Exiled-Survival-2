using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombinedModStat: CombinedStat
{

    public new float Value => modValue;
    public CombinedModStat()
    {
        recalculateValue();
    }
    public CombinedModStat(float baseValue, float increaseValue, List<float> moreValue): base(baseValue, increaseValue, moreValue)
    {
        this.baseValue = 1;
        this.increaseValue = increaseValue;
        this.moreValues = moreValue;
        recalculateValue();
    }

    public void SetValue(float increaseValue, List<float> moreValue)
    {
        this.increaseValue = increaseValue;
        this.moreValues.Clear();
        foreach (float more in moreValue)
            this.moreValues.Add(more);
        recalculateValue();
    }

    public override void AddBaseValue(float value)
    {
        
    }

    public override void RemoveBaseValue(float value)
    {
        
    }


    protected override void recalculateValue()
    {
        modValue = 1;
        foreach (float value in moreValues)
        {
            modValue *= 1 + (value / 100);
        }
        modValue *= 1 + (IncreaseValue / 100);
    }

    public override float ValueWithAddedParams(float baseValue, float increaseValue, List<float> moreValues) //не учитывает отрицательные значения
    {
        //value = baseValue * (1 + (increaseValue / 100)) * MoreValue;
        float newBaseValue = 1;
        float newModValue = 1;
        foreach (float value in base.moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        foreach (float value in moreValues)
        {
            newModValue *= 1 + (value / 100);
        }
        modValue *= 1 + ((IncreaseValue + increaseValue) / 100);
        return (baseValue + newBaseValue) * ModValue;
    }
}
