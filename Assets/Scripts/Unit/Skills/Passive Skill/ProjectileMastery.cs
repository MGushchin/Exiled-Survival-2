using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMastery : Skill
{
    private Transform selfTransform;
    //Utility

    //Skill params
    private CombinedModStat projectileDamageMod = new CombinedModStat(0, 0, new List<float>());
    private int pierceCount = 0;

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


    public override void ApplyUpgrade(string name, int level)
    {
        switch (name)
        {
            case ("Projectile Mastery"):
                {
                    projectileDamageMod.AddIncreaseValue(20);
                    owner.Stats.AddStat(StatTag.ProjectileDamage, StatModType.Increase, 20);
                }
                break;
            case ("Increased Projectile Damage"):
                {
                    projectileDamageMod.AddIncreaseValue(10);
                    owner.Stats.AddStat(StatTag.ProjectileDamage, StatModType.Increase, 10);
                }
                break;
            case ("Projectile Pierce"):
                {
                    pierceCount++;
                    owner.Stats.AddStat(StatTag.Pierce, StatModType.Base, 1);
                }
                break;
        }
    }
}
