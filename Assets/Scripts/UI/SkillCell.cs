using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor.Experimental.GraphView;

public class SkillCell : MonoBehaviour
{
    public Image Icon;
    public Image Border;
    public TMP_Text Name;
    public TMP_Text Description;
    public TMP_Text Tags;
    private Dictionary<SkillTag, string> tagsNames = new Dictionary<SkillTag, string>
    {
        {SkillTag.Life, "Life" },
        {SkillTag.Movement, "Movement" },
        {SkillTag.Armour, "Armour" },
        {SkillTag.Evasion, "Evasion" },
        {SkillTag.Physical, "Physical" },
        {SkillTag.Elemental, "Elemental" },
        {SkillTag.Fire, "Fire" },
        {SkillTag.Cold, "Cold" },
        {SkillTag.Lightning, "Lightning" },
        {SkillTag.Critical, "Critical" },
        {SkillTag.Projectile, "Projectile" },
        {SkillTag.Area, "Area" },
        {SkillTag.Poison, "Poison" },
        {SkillTag.Attack, "Attack" },
        {SkillTag.Spell, "Spell" },
        {SkillTag.Bleeding, "Bleeding" },
        {SkillTag.DamageOverTime, "Damage over time" }
    };
    private Dictionary<Rarities, Color> borderRariritiesColor = new Dictionary<Rarities, Color>
    {
        { Rarities.Common, Color.white },
        { Rarities.Magic, Color.blue },
        { Rarities.Rare, Color.yellow },
        { Rarities.Mythical, new Color(75, 0, 130) }
    };


    public void SetCell(SkillMod data)
    {
        Icon.sprite = data.Icon;
        Border.color = borderRariritiesColor[data.Rarity];
        Name.text = data.Name;
        Description.text = data.Description;
        string tagsString = "";
        foreach (SkillTag tag in data.Tags)
        {
            if(tagsNames.ContainsKey(tag))
                tagsString += tagsNames[tag] + " ";
            else
            {
                tagsString += tag.ToString() + " ";
                Debug.LogWarning("Tag exception");
            }
        }
        Tags.text = tagsString;
    }
}
