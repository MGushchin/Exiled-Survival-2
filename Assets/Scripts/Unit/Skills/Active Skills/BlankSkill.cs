using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlankSkill : Skill
{
    private struct skillHit
    {
        public Hit hit;
        public SkillSkinChanger skinChanger;
    }
    public GameObject HitPrefab;
    private Transform selfTransform;

    //Utility
    private Queue<skillHit> hitsPool = new Queue<skillHit>();
    private IEnumerator cooldownCoroutine;

    #region SkillParams
    private CombinedStat damageModifier = new CombinedStat(0, 0, new List<float>());
    private CombinedStat attackSpeedModifier = new CombinedStat(0, 0, new List<float>());
    private float baseSkillCooldown = 1;
    private float baseCriticalStrikeChance = 5;
    //Ailments section

    //Utility Params
    #endregion

    #region UtilityLinks
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;

        //Init methods

        //Event subscriptions
        
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            //Setup Hits
            
            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(baseSkillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);

            //Final operations

            return true;
        }
        else
            return false;
    }

    private HitData getHitData()
    {
        HitData hit = owner.Stats.GetHitData(baseCriticalStrikeChance, new List<StatTag>());
        hit.PhysicalDamage *= damageModifier.ModValue;
        return hit;
    }

    private List<skillHit> setupHit(Vector3 castPoint, List<skillHit> hits)
    {
        HitData hit = getHitData();

        //Setup

        return hits;
    }

    private void updateHitPool()
    {
        
    }

    public override void StopUseSkill()
    {
        base.StopUseSkill();
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        foreach (skillHit hitInstance in hitsPool)
        {
            hitInstance.hit.OnFeedbackReceived.RemoveListener(owner.TakeHitFeedback);
            Destroy(hitInstance.hit.gameObject);
        }
        hitsPool.Clear();
        StopCoroutine(cooldownCoroutine);

        //Unsubscribe from events

    }

    private IEnumerator cooldownRecovery(float time)
    {
        while (cooldown > 0)
        {
            yield return new WaitForEndOfFrame();
            cooldown -= Time.deltaTime;
        }
    }

    public override void ApplyUpgrade(string name, int level)
    {
        //base.ApplyUpgrade(name, level);
        switch (name)
        {
            case ("0"):
                {
                    
                }
                break;
            case ("1"):
                {
                    
                }
                break;
            case ("2"):
                {
                    
                }
                break;
            case ("3"):
                {
                    
                }
                break;
            default:
                {

                }
                break;
        }
    }
}
