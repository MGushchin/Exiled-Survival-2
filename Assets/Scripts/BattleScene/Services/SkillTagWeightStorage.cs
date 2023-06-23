using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTagWeightStorage : MonoBehaviour
{
    private Dictionary<SkillTag, float> tagsWeightModifier = new Dictionary<SkillTag, float>();
    private const float weightModifier = 0.1f; //10% increased per added tag

    private void Awake()
    {
        foreach (SkillTag tag in Enum.GetValues(typeof(SkillTag)))
            tagsWeightModifier.Add(tag, 1);
    }

    public void AddTag(SkillTag tag)
    {
        tagsWeightModifier[tag] += weightModifier;
    }

    public float GetTagModifier(SkillTag tag)
    {
        return tagsWeightModifier[tag];
    }
}
