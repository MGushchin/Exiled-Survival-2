using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
public class RarityAffixes
{
    public Rarities Rarity;
    public float Weight;
    public List<Affix> Affixes;
}

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
    [HideInInspector]
    public string Description;
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
    private float weight; //Must be equal to sum of MultiRarityAffixes
    public float Weight => weight;
    [SerializeField]
    private bool baseMod;
    public bool BaseMod => baseMod;

    public List<Affix> Affixes = new List<Affix>();
    [Space]
    [SerializeField]
    private bool multiRarity = false;
    public bool MultiRarity => multiRarity;

    public List<RarityAffixes> MultiRarityAffixes = new List<RarityAffixes>();
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

    public void UpdateMultiRarity(Rarities _rarity)
    {
        if(multiRarity)
        {
            rarity = _rarity;
            Affixes.Clear();
            foreach(RarityAffixes rarityAffix in MultiRarityAffixes)
            {
                if(rarityAffix.Rarity == rarity)
                    foreach(Affix affix in rarityAffix.Affixes)
                    {
                        Affixes.Add(affix);
                    }
            }
        }
    }

    public void UpdateDescription()
    {
        Description = description;
        if (multiRarity)
        {
            for (int i = 0; i < MultiRarityAffixes.Count; i++)
            {
                if (MultiRarityAffixes[i].Rarity == rarity)
                {
                    string oldValue = "(" + "Affix" + (i + 1) + ")";
                    for(int j=0; j <  MultiRarityAffixes[i].Affixes.Count; j++)
                    {
                        oldValue = "(" + "Affix" + (j + 1) + ")";
                        Description = description.Replace(oldValue, MultiRarityAffixes[i].Affixes[j].Value.ToString());
                    }
                    //Description = description.Replace(oldValue, MultiRarityAffixes[i].Affixes[i].Value.ToString());
                }
            }
        }
        else
            for (int i = 0; i < Affixes.Count; i++)
            {
                string oldValue = "(" + "Affix" + (i + 1) + ")";
                Description = description.Replace(oldValue, Affixes[i].Value.ToString());
            }
        //Debug
        if (description == "" || description == null)
        {
            Debug.LogWarning("Automatically generated description");
            Description = "";
            foreach (Affix affix in Affixes)
                switch (affix.ModType)
                {
                    case (StatModType.Base):
                        {
                            Description = "+" + affix.Value.ToString() + " to " + affix.Tag;
                        }
                        break;
                    case (StatModType.Increase):
                        {
                            Description = affix.Value.ToString() + "% increased " + affix.Tag;
                        }
                        break;
                    case (StatModType.More):
                        {
                            Description = affix.Value.ToString() + "% more " + affix.Tag;
                        }
                        break;
                }
        }
    }
}
