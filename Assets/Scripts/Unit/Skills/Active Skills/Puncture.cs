using Statuses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puncture : Skill
{
    private struct skillHit
    {
        public Hit hit;
        public DisappearingDelay Delay;
        public SpriteRenderer Renderer;
    }
    public GameObject HitPrefab;
    private Transform selfTransform;

    //Utility
    private Queue<skillHit> hitsPool = new Queue<skillHit>();
    private IEnumerator cooldownCoroutine;

    #region SkillParams
    //private CombinedStat damageModifier = new CombinedStat(5, 0, new List<float>());
    //private CombinedStat attackSpeedModifier = new CombinedStat(0, 0, new List<float>());
    private CombinedStat areaOfEffectModifier = new CombinedStat(1, 0, new List<float>());
    private CombinedStat bleedingChanceModifier = new CombinedStat(100, 0, new List<float>());
    private CombinedStat bleedingDamageModifier = new CombinedStat(2, 0, new List<float>());
    private CombinedStat bleedingDurationModifier = new CombinedStat(8, 0, new List<float>());
    private float baseSkillCooldown = 2;
    private float multistrikeChance = 0;
    //Ailments section

    //Utility Params

    #endregion

    #region UtilityLinks
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    private float resultAreaOfEffect => areaOfEffectModifier.Value * owner.Stats.GetAdvancedStat(StatTag.AreaOfEffect).ModValue;
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private float resultMultistrikeChance => multistrikeChance + owner.Stats.GetAdvancedStat(StatTag.MultistrikeChance).Value;
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;
        damageModifier = new CombinedStat(4, 0, new List<float>());
        attackSpeedModifier = new CombinedStat(0, 0, new List<float>());
        //Init methods
        hitTags = new List<StatTag>() { StatTag.AttackDamage, StatTag.PhysicalDamage, StatTag.BleedingDamage};
        updateHitPool();
        updateAreaOfEffectPool(1);
        updateAttackSpeedPool(1);
        //Event subscriptions
        owner.Stats.OnStatChanged[StatTag.AreaOfEffect].AddListener(updateAreaOfEffectPool);
        owner.Stats.OnStatChanged[StatTag.CooldownRecovery].AddListener(updateAttackSpeedPool);
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            //Setup Hits
            int garantedMultistrikes = (int)(resultMultistrikeChance / 100);
            int hitsCount = 1 + garantedMultistrikes;

            if (Random.Range(1, 100) <= (resultMultistrikeChance - garantedMultistrikes * 100))
                hitsCount++;

            List<skillHit> hitsToSetup = new List<skillHit>();
            for (int i = 0; i < hitsCount; i++)
                hitsToSetup.Add(hitsPool.Dequeue());

            setupHit(castPoint, hitsToSetup);

            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(baseSkillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);

            //Final operations
            for (int i = 0; i < hitsToSetup.Count; i++)
                hitsPool.Enqueue(hitsToSetup[i]);

            return true;
        }
        else
            return false;
    }

    protected override HitData getHitData()
    {
        #region StatsMethod
        HitData hit = new HitData(owner);
        hit.Ally = owner.Ally;
        CombinedModStat modStat = new CombinedModStat();
        modStat.AddIncreaseValue(owner.Stats.GetAdvancedStat(StatTag.damage).IncreaseValue);
        modStat.AddMoreValue(owner.Stats.GetAdvancedStat(StatTag.damage).MoreValue);

        foreach (StatTag tag in hitTags)
        {
            modStat.AddIncreaseValue(owner.Stats.GetAdvancedStat(tag).IncreaseValue);
            modStat.AddMoreValue(owner.Stats.GetAdvancedStat(tag).MoreValues);
        }

        hit.PhysicalDamage = owner.Stats.GetAdvancedStat(StatTag.PhysicalDamage).ValueWithAddedParams(modStat.Value) * damageMultiplier;//����������
        hit.FireDamage = owner.Stats.GetAdvancedStat(StatTag.FireDamage).ValueWithAddedParams(modStat.Value) * damageMultiplier;//����������
        hit.ColdDamage = owner.Stats.GetAdvancedStat(StatTag.ColdDamage).ValueWithAddedParams(modStat.Value) * damageMultiplier;//����������
        hit.LightningDamage = owner.Stats.GetAdvancedStat(StatTag.LightningDamage).ValueWithAddedParams(modStat.Value) * damageMultiplier;//����������
        hit.CriticalStrikeChance = owner.Stats.GetAdvancedStat(StatTag.CriticalStrikeChance).ValueWithAddedParams(criticalStrikeChanceModifier); //����������
        hit.CriticalStrikeMultiplier = owner.Stats.GetAdvancedStat(StatTag.CriticalStrikeMultiplier).Value;

            int garantedStacks = (int)((bleedingChanceModifier.Value + owner.Stats.GetAdvancedStat(StatTag.BleedingChance).Value) / 100);
            int stacksCount = garantedStacks;
            if (Random.Range(1, 100f) <= (bleedingChanceModifier.Value + owner.Stats.GetAdvancedStat(StatTag.BleedingChance).Value - garantedStacks * 100))
                stacksCount++;

            for (int i = 0; i < stacksCount; i++)
            {
                //Bleeding
                CombinedStat bleedingDamageMod = new CombinedStat();
                bleedingDamageMod.AddIncreaseValue(owner.Stats.GetAdvancedStat(StatTag.BleedingDamage).IncreaseValue);
                bleedingDamageMod.AddMoreValue(owner.Stats.GetAdvancedStat(StatTag.BleedingDamage).MoreValue); //����������
                float bleedingDamageValue = bleedingDamageModifier.ValueWithAddedParams(bleedingDamageMod);
                Status status = new Status(StatusType.Bleeding, owner.Stats.GetAdvancedStat(StatTag.BleedingDuration).Value, (int)bleedingDamageValue); //sender ���������, int ��������������
                status.damage = bleedingDamageValue;
                hit.InflicktedStatuses.Add(owner.Stats.GetStatus(StatusType.Bleeding)); //���������� �� ����� � �������
            }
        #endregion

        hit.Tags = skillTags;
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
            Vector3 modifiedcastPoint = Vector3.ClampMagnitude(Vector3.Normalize(castPoint - transform.position), attackRange);
            hits[i].hit.SelfTransform.position = selfTransform.position + (Vector3.Normalize(castPoint - transform.position)) + offset;
            hits[i].hit.SelfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - selfTransform.position.y, castPoint.x - selfTransform.position.x) * Mathf.Rad2Deg/* - 90*/)); //����������
            offset = new Vector3(Random.Range(-offsetMagnitude, offsetMagnitude), Random.Range(-offsetMagnitude, offsetMagnitude), 0);
            //Set Hits
            hits[i].hit.SetHit(hit);
            hits[i].Renderer.flipY = flip;
            flip = !flip;
            hits[i].hit.SetActiveHit(true);
        }
        return hits;
    }

    private void updateHitPool()
    {
        int garantedMultistrikes = (int)(multistrikeChance / 100);
        int hitsCount = (int)((1 / resultSkillCooldown)) * (garantedMultistrikes + 2) + 1;
        Debug.Log("Hits count " + hitsCount);
        for (int i = hitsPool.Count; i < hitsCount; i++)
        {
            Debug.Log("CreatedHit");
            GameObject temporal = Instantiate(HitPrefab);
            skillHit temporalHit = new skillHit();
            temporalHit.hit = temporal.GetComponent<Hit>();
            temporalHit.Delay = temporal.GetComponent<DisappearingDelay>();
            temporalHit.Renderer = temporal.GetComponent<SpriteRenderer>();
            temporalHit.hit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
            hitsPool.Enqueue(temporalHit);
            temporal.SetActive(false);
        }
    }

    private void updateAreaOfEffectPool(float area)
    {
        foreach (skillHit hit in hitsPool)
        {
            hit.hit.gameObject.transform.localScale = new Vector3(resultAreaOfEffect, resultAreaOfEffect, 1);
        }
    }

    private void updateAttackSpeedPool(float area)
    {
        float dissapearingDelay = resultSkillCooldown * 0.8f;
        foreach (skillHit hit in hitsPool)
        {
            hit.Delay.SetDelay(dissapearingDelay);
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
        owner.Stats.OnStatChanged[StatTag.AreaOfEffect].RemoveListener(updateAreaOfEffectPool);
        owner.Stats.OnStatChanged[StatTag.CooldownRecovery].RemoveListener(updateAttackSpeedPool);
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
            case ("Cleave Damage"):
                {
                    damageModifier.AddIncreaseValue(10);
                }
                break;
            case ("Cleave Attack Speed"):
                {
                    attackSpeedModifier.AddIncreaseValue(10);
                }
                break;
            case ("Cleave Area"):
                {
                    areaOfEffectModifier.AddIncreaseValue(10);
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
