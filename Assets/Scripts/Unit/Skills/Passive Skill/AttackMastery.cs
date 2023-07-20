using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMastery : Skill
{
    private Transform selfTransform;
    //Utility

    //Skill params
    private CombinedStat attackDamage = new CombinedStat(0, 0, new List<float>());
    private CombinedStat multistrikeChance = new CombinedStat(0, 0, new List<float>());
    private CombinedStat vampirism = new CombinedStat(0, 0, new List<float>());

    //Ruthless hits
    private bool canDealRuthless = false;
    private float ruthlessHitDamageMultiplier = 1.8f;
    private int ruthlessCounter = 0;
    private int hitsToRuthless = 3;

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        cooldown = 0;
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
        //if (canDealRuthless)
        //    owner.Stats.OnPreparingHit.RemoveListener(RuthlessHitsCount);
    }


    public override void ApplyUpgrade(SkillMod mod)
    {
        switch (mod.name)
        {
            case ("Attack Mastery"):
                {
                    attackDamage.AddIncreaseValue(20);
                    owner.Stats.AddStat(StatTag.AttackDamage, StatModType.Increase, 20);
                }
                break;
            case ("Attack Damage"):
                {
                    attackDamage.AddIncreaseValue(10);
                    owner.Stats.AddStat(StatTag.AttackDamage, StatModType.Increase, 10);
                }
                break;
            case ("Attack Multistrike"):
                {
                    multistrikeChance.AddBaseValue(10);
                    owner.Stats.AddStat(StatTag.MultistrikeChance, StatModType.Base, 10);
                }
                break;
            case ("Ruthless hits"):
                {
                    canDealRuthless = true;
                    //owner.Stats.OnPreparingHit.AddListener(RuthlessHitsCount);
                }
                break;
            case ("Attack Vampirism"):
                {
                    vampirism.AddBaseValue(1);
                    owner.Stats.AddStat(StatTag.LifeLeech, StatModType.Base, 1);
                }
                break;
            default:
                {
                    Debug.LogError("Default exception");
                }
                break;
        }
    }

    public void RuthlessHitsCount(HitData data, List<StatTag> tags)
    {
        if (tags.Contains(StatTag.AttackDamage))
        {
            ruthlessCounter++;
            if (ruthlessCounter >= hitsToRuthless)
            {
                data.PhysicalDamage *= ruthlessHitDamageMultiplier;
                data.FireDamage *= ruthlessHitDamageMultiplier;
                data.ColdDamage *= ruthlessHitDamageMultiplier;
                data.LightningDamage *= ruthlessHitDamageMultiplier;
            }
        }
    }
}
