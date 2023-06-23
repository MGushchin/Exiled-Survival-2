using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalStrikeMastery : Skill
{
    private Transform selfTransform;
    //Utility

    //Skill params
    private CombinedStat criticalStrikeChance = new CombinedStat(0, 0, new List<float>());
    private CombinedStat criticalStrikeMultiplier = new CombinedStat(0, 0, new List<float>());

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
            case ("Critical Strike Mastery"):
                {
                    criticalStrikeChance.AddIncreaseValue(20);
                    owner.Stats.AddStat(StatTag.CriticalStrikeChance, StatModType.Increase, 20);
                    criticalStrikeMultiplier.AddBaseValue(20);
                    owner.Stats.AddStat(StatTag.CriticalStrikeMultiplier, StatModType.Base, 20);
                }
                break;
            case ("Increased Critical Strike Chance"):
                {
                    criticalStrikeChance.AddIncreaseValue(40);
                    owner.Stats.AddStat(StatTag.CriticalStrikeChance, StatModType.Increase, 40);
                }
                break;
            case ("Increased Critical Strike Multiplier"):
                {
                    criticalStrikeMultiplier.AddBaseValue(20);
                    owner.Stats.AddStat(StatTag.CriticalStrikeMultiplier, StatModType.Base, 20);
                }
                break;

        }
    }
}
