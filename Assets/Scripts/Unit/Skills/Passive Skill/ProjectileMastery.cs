using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMastery : Skill
{
    private Transform selfTransform;
    //Utility

    //Skill params
    private List<Affix> affixes = new List<Affix>();

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        cooldown = 0;
        Debug.Log(gameObject.name);
        Debug.Log(skillOwner);
        Debug.Log(owner);
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        return true;
    }

    public override void StopUseSkill()
    {
        base.StopUseSkill();
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
    }


    public override void ApplyUpgrade(SkillMod mod)
    {
        Debug.Log("ApplyUpgrade " + mod.name);
        foreach (Affix affix in mod.Affixes)
        {
            Debug.Log("Affix " + affix.Tag + " " + affix.ModType + " " + affix.Value);
            affixes.Add(affix);
            owner.Stats.AddStat(affix.Tag, affix.ModType, affix.Value);
        }
        switch (mod.name)
        {
            default:
                {
                    
                }
                break;
        }
    }
}
