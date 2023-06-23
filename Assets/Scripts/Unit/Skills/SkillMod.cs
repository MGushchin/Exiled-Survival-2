using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public struct SkillModRequirements
{
    public string Name;
    public int Level;
}

//[System.Serializable]
//public struct SkillRequirements
//{
//    public Skill skill;
//    public int level;
//}

[System.Serializable]
public struct Affix
{
    public StatTag Tag;
    public StatModType ModType;
    public float Value;

    public Affix(StatTag tag, StatModType modType, float value)
    {
        Tag = tag;
        ModType = modType;
        Value = value;
    }
}

[CreateAssetMenu(fileName = "Skill Mod", menuName = "ScriptableObjects/Skill Mod", order = 1)]
public class SkillMod : ScriptableObject
{
    [SerializeField]
    private string modName;
    public string Name => modName;
    [SerializeField]
    private Sprite icon;
    public Sprite Icon => icon;
    [SerializeField]
    [TextArea(4, 8)]
    private string description;
    public string Description => description;
    [SerializeField]
    private int maximumLevel = 0;
    public int MaximumLevel => maximumLevel;
    [SerializeField]
    private int level = 0;
    public int Level => level;
    [SerializeField]
    private Rarities rarity;
    public Rarities Rarity => rarity;
    [SerializeField]
    private List<SkillTag> tags = new List<SkillTag>();
    public List<SkillTag> Tags => tags;
    [SerializeField]
    private float weight;
    public float Weight => weight;
    [SerializeField]
    private bool baseMod;
    public bool BaseMod => baseMod;
    [Space]
    //Legacy
    public List<float> Values = new List<float>(); // Ограничить доступ
    public List<Affix> Affixes = new List<Affix>();
    [Space]
    public List<SkillModRequirements> SkillsModsRequirements = new List<SkillModRequirements>();
    [Space]
    public List<string> SkillModsForExclusion = new List<string>();

    //Utility
    private Skill parentSkill;
    [HideInInspector]
    public Skill ParentSkill => parentSkill;

    public void UpgradeLevel()
    {
        level++;
    }

    public void SetParentSkill(Skill parent)
    {
        parentSkill = parent;
    }

    public void UpdateDescription()
    {
        //Legacy
        for (int i = 0; i < Values.Count; i++)
        {
            string oldValue = "(" + (i + 1) + ")";
            description = description.Replace(oldValue, Values[i].ToString());
        }
        for (int i = 0; i < Affixes.Count; i++)
        {
            string oldValue = "(" + "Affix" +(i + 1) + ")";
            description = description.Replace(oldValue, Affixes[i].Value.ToString());
        }
        //Debug
        if (description == "" || description == null)
        {
            Debug.LogWarning("Automatically generated description");
            description = "";
            foreach (Affix affix in Affixes)
                switch (affix.ModType)
                {
                    case (StatModType.Base):
                        {
                            description = "+" + affix.Value.ToString() + " to " + affix.Tag;
                        }
                        break;
                    case (StatModType.Increase):
                        {
                            description = affix.Value.ToString() + "% increased " + affix.Tag;
                        }
                        break;
                    case (StatModType.More):
                        {
                            description = affix.Value.ToString() + "% more " + affix.Tag;
                        }
                        break;
                }
        }
    }
}
