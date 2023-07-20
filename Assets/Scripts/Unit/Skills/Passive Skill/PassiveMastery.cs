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
        private List<Affix> affixes = new List<Affix>();

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
        }


        public override void ApplyUpgrade(SkillMod mod)
        {
            Debug.Log("ApplyUpgrade " + mod.name);
            foreach(Affix affix in mod.Affixes)
            {
                Debug.Log("Affix " + affix.Tag + " " + affix.ModType + " " + affix.Value);
                affixes.Add(affix);
                owner.Stats.AddStat(affix.Tag, affix.ModType, affix.Value);
            }
            switch (mod.Name)
            {
                //case ("Maximum Life"):
                //    {
                //        maximumLifeMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.life, StatModType.Base, mod.Values[0]);
                //    }
                //    break;
                //case ("Life Regeneration"):
                //    {
                //        lifeRegenerationMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.lifeRegeneration, StatModType.Base, mod.Values[0]);
                //    }
                //    break;
                //case ("Physical Damage"):
                //    {
                //        physicalDamageMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.PhysicalDamage, StatModType.Base, mod.Values[0]);

                //    }
                //    break;
                //case ("Fire Damage"):
                //    {
                //        fireDamageMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.FireDamage, StatModType.Base, mod.Values[0]);

                //    }
                //    break;
                //case ("Cold Damage"):
                //    {
                //        coldDamageMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.ColdDamage, StatModType.Base, mod.Values[0]);

                //    }
                //    break;
                //case ("Lightning Damage"):
                //    {
                //        lightningDamageMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.LightningDamage, StatModType.Base, mod.Values[0]);

                //    }
                //    break;
                //case ("Armour"):
                //    {
                //        armourMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.Armour, StatModType.Base, mod.Values[0]);
                //    }
                //    break;
                //case ("Attack Damage"):
                //    {
                //        armourMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.Armour, StatModType.Base, mod.Values[0]);
                //    }
                //    break;
                //case ("Attack Damage 2"):
                //    {
                //        armourMod.AddBaseValue(mod.Values[0]);
                //        owner.Stats.AddStat(StatTag.Armour, StatModType.Base, mod.Values[0]);
                //    }
                //    break;
                default:
                    {
                        //Debug.LogError(string.Format("Default exception. Skill mod {0}", mod.name));
                    }break;
            }
        }
    }
}