using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMastery : Skill
{
    private Transform selfTransform;
    //Utility

    //Skill params
    private List<Affix> affixes = new List<Affix>();

    //Ruthless hits
    //private bool canDealRuthless = false;
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

    //public void RuthlessHitsCount(HitData data, List<StatTag> tags)
    //{
    //    if (tags.Contains(StatTag.AttackDamage))
    //    {
    //        ruthlessCounter++;
    //        if (ruthlessCounter >= hitsToRuthless)
    //        {
    //            //data.PhysicalDamage *= ruthlessHitDamageMultiplier;
    //            //data.FireDamage *= ruthlessHitDamageMultiplier;
    //            //data.ColdDamage *= ruthlessHitDamageMultiplier;
    //            //data.LightningDamage *= ruthlessHitDamageMultiplier;
    //            foreach (CombinedStat damageType in data.Damage.Values)
    //            {
    //                damageType.AddMoreValue(ruthlessHitDamageMultiplier);
    //            }
    //        }
    //    }
    //}
}
