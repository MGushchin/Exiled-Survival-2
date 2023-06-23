using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalDamageMastery : Skill
{
    private Transform selfTransform;
    //Utility

    //Skill params
    private CombinedStat physicalDamage = new CombinedStat(0, 0, new List<float>());
    private CombinedStat elementalDamage = new CombinedStat(0, 0, new List<float>());
    private CombinedStat bleedingChance = new CombinedStat(0, 0, new List<float>());
    private CombinedStat bleedingDamage = new CombinedStat(0, 0, new List<float>());

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
        switch (mod.name)
        {
            case ("Physical Damage Mastery"):
                {
                    physicalDamage.AddIncreaseValue(20);
                    owner.Stats.AddStat(StatTag.PhysicalDamage, StatModType.Increase, 20);
                }
                break;
            case ("Increased Physical Damage"):
                {
                    physicalDamage.AddIncreaseValue(10);
                    owner.Stats.AddStat(StatTag.PhysicalDamage, StatModType.Increase, 10);
                }
                break;
            case ("Pure Power"):
                {
                    physicalDamage.AddIncreaseValue(20);
                    owner.Stats.AddStat(StatTag.PhysicalDamage, StatModType.Increase, 20);
                    elementalDamage.RemoveIncreaseValue(20);
                    owner.Stats.RemoveStat(StatTag.FireDamage, StatModType.Increase, 20);
                    owner.Stats.RemoveStat(StatTag.ColdDamage, StatModType.Increase, 20);
                    owner.Stats.RemoveStat(StatTag.LightningDamage, StatModType.Increase, 20);
                }
                break;
            case ("Chance to bleed"):
                {
                    bleedingChance.AddBaseValue(40);
                    owner.Stats.AddStat(StatTag.BleedingChance, StatModType.Base, 40);
                }
                break;
            case ("Bleeding damage"):
                {
                    bleedingDamage.AddBaseValue(20);
                    owner.Stats.AddStat(StatTag.BleedingDamage, StatModType.Increase, 20);
                }
                break;
            default:
                {
                    Debug.LogWarning("Default exception");
                }break;
        }
    }
}
