using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Skills
{
    public enum PassiveMasteryPassives
    {
        MaximumLife,
        LifeRegeneration,
        PhysicalDamage,
        FireDamage,
        ColdDamage,
        LightningDamage,
        Armour
    }

    public class PassiveMastery : Skill
    {
        private Transform selfTransform;
        //Utility

        //Skill params
        private CombinedStat maximumLifeMod = new CombinedStat(0, 0, new List<float>());
        private CombinedStat lifeRegenerationMod = new CombinedStat(0, 0, new List<float>());
        private CombinedStat physicalDamageMod = new CombinedStat(0, 0, new List<float>());
        private CombinedStat fireDamageMod = new CombinedStat(0, 0, new List<float>());
        private CombinedStat coldDamageMod = new CombinedStat(0, 0, new List<float>());
        private CombinedStat lightningDamageMod = new CombinedStat(0, 0, new List<float>());
        private CombinedStat armourMod = new CombinedStat(0, 0, new List<float>());

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
                case ("Maximum Life"):
                    {
                        maximumLifeMod.AddBaseValue(5);
                        owner.Stats.AddStat(StatTag.life, StatModType.Base, 5);
                    }
                    break;
                case ("Life Regeneration"):
                    {
                        lifeRegenerationMod.AddBaseValue(1);
                        owner.Stats.AddStat(StatTag.lifeRegeneration, StatModType.Base, 1);
                    }
                    break;
                case ("Physical Damage"):
                    {
                        physicalDamageMod.AddBaseValue(2);
                        owner.Stats.AddStat(StatTag.PhysicalDamage, StatModType.Base, 2);

                    }
                    break;
                case ("Fire Damage"):
                    {
                        fireDamageMod.AddBaseValue(2);
                        owner.Stats.AddStat(StatTag.FireDamage, StatModType.Base, 2);

                    }
                    break;
                case ("Cold Damage"):
                    {
                        coldDamageMod.AddBaseValue(2);
                        owner.Stats.AddStat(StatTag.ColdDamage, StatModType.Base, 2);

                    }
                    break;
                case ("Lightning Damage"):
                    {
                        lightningDamageMod.AddBaseValue(2);
                        owner.Stats.AddStat(StatTag.LightningDamage, StatModType.Base, 2);

                    }
                    break;
                case ("Armour"):
                    {
                        armourMod.AddBaseValue(5);
                        owner.Stats.AddStat(StatTag.Armour, StatModType.Base, 5);

                    }
                    break;
            }
        }
    }
}