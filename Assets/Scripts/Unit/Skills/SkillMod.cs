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
    private bool baseMod;
    public bool BaseMod => baseMod;
    [Space]
    public List<float> Values = new List<float>(); // Ограничить доступ
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
        for(int i=0; i < Values.Count; i++)
        {
            string oldValue = "(" + (i + 1) + ")";
            description = description.Replace(oldValue, Values[i].ToString());

        }
    }
}
