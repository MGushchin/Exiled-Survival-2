using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skills
{
    public class LifeMastery : Skill
    {
        private Transform selfTransform;
        //Utility

        //Skill params
        private CombinedStat maximumLifeMod = new CombinedStat(0, 0, new List<float>());
        private CombinedStat lifeRegenerationMod = new CombinedStat(0, 0, new List<float>());

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
                case ("Life Mastery"):
                    {
                        maximumLifeMod.AddIncreaseValue(10);
                        owner.Stats.AddStat(StatTag.life, StatModType.Increase, 10);
                    }
                    break;
                case ("Increased Maximum Life"):
                    {
                        maximumLifeMod.AddIncreaseValue(10);
                        owner.Stats.AddStat(StatTag.life, StatModType.Increase, 10);
                    }
                    break;
                case ("Life Regeneration"):
                    {
                        lifeRegenerationMod.AddBaseValue(1);
                        owner.Stats.AddStat(StatTag.lifeRegeneration, StatModType.Base, 1);

                    }
                    break;
            }
        }
    }
}
