using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigNovaExplosion : Skill
{
    private struct skillHit
    {
        public Hit hit;
        public CircleDangerZoneRenderer DangerZone;
        public DisappearingDelay Delay;
    }
    public GameObject HitPrefab;
    public GameObject DangerZonePrefab;

    private Transform selfTransform;

    //Utility
    private Queue<skillHit> hitsPool = new Queue<skillHit>();
    private IEnumerator cooldownCoroutine;
    private IEnumerator castingCoroutine;

    #region SkillParams
    private float baseSkillCooldown = 10;
    [SerializeField]
    private float castTime = 3; //Перенести в будущем в attackspeedmod или иное
    private CombinedStat areaOfEffectModifier = new CombinedStat(5, 0, new List<float>());
    //Ailments section

    //Utility Params
    #endregion

    #region UtilityLinks
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private float resultAreaOfEffect => areaOfEffectModifier.Value * owner.Stats.GetAdvancedStat(StatTag.AreaOfEffect).ModValue;
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;

        //Init methods
        hitTags = new List<StatTag>() { StatTag.AreaDamage, StatTag.AttackDamage, StatTag.FireDamage };
        updateHitPool();
        updateAreaOfEffectPool(1);
        //Event subscriptions

    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            //Setup Hits
            List<skillHit> hitsToSetup = new List<skillHit>();
            hitsToSetup.Add(hitsPool.Dequeue());
            setupHit(castPoint, hitsToSetup);

            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(baseSkillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);

            //Final operations
            for (int i = 0; i < hitsToSetup.Count; i++) //Переписать списки мб
                hitsPool.Enqueue(hitsToSetup[i]);
            return true;
        }
        else
            return false;
    }

    protected override HitData getHitData()
    {
        HitData hit = base.getHitData();
        return hit;
    }

    private List<skillHit> setupHit(Vector3 castPoint, List<skillHit> hits)
    {
        HitData hit = getHitData();

        //Setup
        for (int i = 0; i < hits.Count; i++)
        {
            //Set Hits
            hits[i].hit.SetHit(hit);
            //hits[i].hit.SetActiveHit(true);
            castingCoroutine = casting(hits[i]);
            StartCoroutine(castingCoroutine);
        }
        return hits;
    }

    private IEnumerator casting(skillHit hit)
    {
        hit.DangerZone.SetActive(areaOfEffectModifier.Value, castTime);
        yield return new WaitForSeconds(castTime);
        hit.hit.SetActiveHit(true);
    }

    private void updateHitPool()
    {
        int hitsCount = 1;
        for (int i = hitsPool.Count; i < hitsCount; i++)
        {
            GameObject temporal = Instantiate(HitPrefab);
            skillHit temporalHit = new skillHit();
            temporalHit.hit = temporal.GetComponent<Hit>();
            temporalHit.Delay = temporal.GetComponent<DisappearingDelay>();
            temporalHit.hit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
            temporal.transform.parent = transform;
            temporal.SetActive(false);

            GameObject temporalDangerZone = Instantiate(DangerZonePrefab);
            temporalDangerZone.transform.parent = transform;
            temporalHit.DangerZone = temporalDangerZone.GetComponent<CircleDangerZoneRenderer>();
            temporalDangerZone.SetActive(false);

            temporal.transform.localPosition = Vector3.zero;
            temporalDangerZone.transform.localPosition = Vector3.zero;

            hitsPool.Enqueue(temporalHit);
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
            Destroy(hitInstance.DangerZone.gameObject);
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
        switch (mod.name)
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

    private void updateAreaOfEffectPool(float area)
    {
        foreach (skillHit hit in hitsPool)
        {
            hit.hit.gameObject.transform.localScale = new Vector3(resultAreaOfEffect, resultAreaOfEffect, 1);
        }
    }
}
