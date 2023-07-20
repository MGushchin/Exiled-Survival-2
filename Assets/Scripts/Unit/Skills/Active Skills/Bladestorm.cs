using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bladestorm : Skill
{
    private struct skillHit
    {
        public HitOverTime hit;
        public DisappearingDelay Delay;
        public SkillSkinChanger skinChanger;
    }
    public GameObject HitPrefab;
    private Transform selfTransform;

    //Utility
    private Queue<skillHit> hitsPool = new Queue<skillHit>();
    private IEnumerator cooldownCoroutine;

    private bool following = false;
    private bool homing = false;

    #region SkillParams
    //private CombinedStat damageModifier = new CombinedStat(4, 0, new List<float> { 0.5f }); //50% damage mult
    //private CombinedStat attackSpeedModifier = new CombinedStat(0, 0, new List<float>());
    private CombinedStat areaOfEffectModifier = new CombinedStat(3, 0, new List<float>());
    private CombinedStat durationModifier = new CombinedStat(3, 0, new List<float>());
    //private float baseSkillCooldown = 3;
    //private float baseCriticalStrikeChance = 5;
    private float multistrikeChance = 0;
    //Ailments section

    //Utility Params
    #endregion

    #region UtilityLinks
    private float resultSkillCooldown => skillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private float resultDuration => durationModifier.Value;
    private float resultAreaOfEffect => areaOfEffectModifier.Value * owner.Stats.GetAdvancedStat(StatTag.AreaOfEffect).ModValue;
    private float resultMultistrikeChance => multistrikeChance + owner.Stats.GetAdvancedStat(StatTag.MultistrikeChance).Value;
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;

        //Init Skill params
        damageModifier = new CombinedStat(4, 0, new List<float> { 0.5f }); //4 base damage, 50% damage mult
        skillCooldown = 3;
        criticalStrikeChanceModifier = new CombinedStat(5, 0, new List<float>());
        criticalStrikeMultiplierModifier = new CombinedStat(150, 0, new List<float>());

        //Init methods
        hitTags = new List<StatTag>() { StatTag.AttackDamage, StatTag.PhysicalDamage, StatTag.AreaDamage };
        updateHitPool();
        updateAreaOfEffectPool(1);

        //Event subscriptions

    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            //Setup Hits
            int garantedMultistrikes = (int)(resultMultistrikeChance / 100);
            int hitsCount = 1 + garantedMultistrikes;
            if (Random.Range(0, 100) <= (resultMultistrikeChance - garantedMultistrikes * 100))
            {
                hitsCount++;
            }
            List<skillHit> hitsToSetup = new List<skillHit>();
            for (int i = 0; i < hitsCount; i++)
                hitsToSetup.Add(hitsPool.Dequeue());
            setupHit(castPoint, hitsToSetup);
            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(skillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);

            //Final operations
            for (int i = 0; i < hitsToSetup.Count; i++)
                hitsPool.Enqueue(hitsToSetup[i]);

            return true;
        }
        else
            return false;
    }

    protected override HitData getHitData(/*float baseCriticalStrikeChance*/)
    {
        HitData hit = base.getHitData();
        //HitData hit = owner.Stats.GetHitData(baseCriticalStrikeChance, new List<StatTag>());
        //hit.Tags = skillTags;
        //hit.PhysicalDamage = damageModifier.ValueWithAddedParams(hit.PhysicalDamage);
        //hit.FireDamage *= damageModifier.ModValue;
        //hit.ColdDamage *= damageModifier.ModValue;
        //hit.LightningDamage *= damageModifier.ModValue;
        return hit;
    }

    private List<skillHit> setupHit(Vector3 castPoint, List<skillHit> hits)
    {
        //Setup
        HitData hit = getHitData();
        float offsetMagnitude = resultAreaOfEffect / 2;
        bool flip = true;
        if (Random.Range(0, 2) == 0)
            flip = false;
        Vector3 offset = new Vector3(0, 0, 0);
        for (int i = 0; i < hits.Count; i++)
        {
            //Setup transform data
            if(!following)
                hits[i].hit.SelfTransform.position = selfTransform.position + Vector3.Normalize(castPoint - transform.position) + offset;
            hits[i].Delay.SetDelay(resultDuration);
            hits[i].hit.SelfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - selfTransform.position.y, castPoint.x - selfTransform.position.x) * Mathf.Rad2Deg/* - 90*/)); //Переписать
            offset = new Vector3(Random.Range(-offsetMagnitude, offsetMagnitude), Random.Range(-offsetMagnitude, offsetMagnitude), 0);
            //Set Hits
            hits[i].hit.SetHit(hit);
            flip = !flip;
            hits[i].hit.SetActiveHit(true);
        }
        return hits;
    }

    private void updateHitPool()
    {
        int garantedMultistrikes = (int)(multistrikeChance / 100);
        int hitsCount = (int)((1 / resultSkillCooldown)) * (garantedMultistrikes + 2);
        int maximumBladestorms = (int)((1 / resultCooldownRecoverySpeed) * resultDuration) + 1;

        if(hitsPool.Count < maximumBladestorms)
        {
            for(int i = hitsPool.Count; i < maximumBladestorms; i++)
            {
                GameObject temporal = Instantiate(HitPrefab);
                skillHit temporalHit = new skillHit();
                temporalHit.hit = temporal.GetComponent<HitOverTime>();
                temporalHit.skinChanger = temporal.GetComponent<SkillSkinChanger>();
                temporalHit.Delay = temporal.GetComponent<DisappearingDelay>();
                temporalHit.hit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
                hitsPool.Enqueue(temporalHit);
                temporal.SetActive(false);
                if(homing)
                {
                    Homing homing = temporal.AddComponent(typeof(Homing)) as Homing;
                    homing.SetAlly(owner.Ally);
                    homing.SetSpeed(1);
                }
                if(following)
                {
                    temporal.transform.parent = owner.gameObject.transform;
                    temporal.transform.position = owner.gameObject.transform.position;
                }
            }
        }
    }

    private void updateAreaOfEffectPool(float area)
    {
        foreach (skillHit hit in hitsPool)
        {
            hit.hit.gameObject.transform.localScale = new Vector3(resultAreaOfEffect, resultAreaOfEffect, 1);
        }
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

    public override void ApplyUpgrade(SkillMod mod)
    {
        //base.ApplyUpgrade(name, level);
        switch (mod.Name)
        {
            case ("Bladestorm Damage"):
                {
                    damageModifier.AddIncreaseValue(mod.Affixes[0].Value);
                }
                break;
            case ("Bladestorm Area"):
                {
                    areaOfEffectModifier.AddIncreaseValue(mod.Affixes[0].Value);
                }
                break;
            case ("Bladestorm Duration"):
                {
                    durationModifier.AddIncreaseValue(mod.Affixes[0].Value);
                }
                break;
            case ("Bladestorm Homing"):
                {
                    homing = true;
                    foreach(skillHit hit in hitsPool)
                    {
                        Homing homing = hit.hit.gameObject.AddComponent(typeof(Homing)) as Homing;
                        homing.SetAlly(owner.Ally);
                        homing.SetSpeed(1);
                    }
                }
                break;
            case ("Bladestorm Follow"):
                {
                    following = true;
                    foreach (skillHit hit in hitsPool)
                    {
                        hit.hit.gameObject.transform.parent = owner.gameObject.transform;
                        hit.hit.gameObject.transform.position = owner.gameObject.transform.position;
                    }
                }
                break;
            default:
                {
                    Debug.Log("default");
                }
                break;
        }
    }
}
