using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAttackWithIndication : Skill
{
    public GameObject HitPrefab;
    public GameObject DangerZonePrefab;
    private Hit attackHit;
    private HitTimeMultiplierSetter timeSetter;
    private Transform selfTransform;
    //Utility
    private IEnumerator attackCoroutine;
    private VectorDangerZoneRenderer DangerZone;
    private Vector2 dangerZoneSize = new Vector2();
    //private IEnumerator cooldownCoroutine;
    //Skill params
    //private float skillCooldown = 1;
    private float baseSkillCooldown = 1;
    //private float baseCriticalStrikeChance = 5;

    #region UtilityLinks
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value)); // Добавить модификатор скорости атаки
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        base.InitSKill(skillOwner);
        selfTransform = gameObject.transform;
        DangerZone = Instantiate(DangerZonePrefab, transform).GetComponent<VectorDangerZoneRenderer>();
        GameObject hit = Instantiate(HitPrefab, gameObject.transform);
        attackHit = hit.GetComponent<Hit>();
        timeSetter = hit.GetComponent<HitTimeMultiplierSetter>();
        attackHit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
        attackHit.SetActiveHit(false);
        hit.transform.localScale = new Vector3(hit.transform.localScale.x * owner.Stats.GetStat(StatTag.AreaOfEffect), hit.transform.localScale.y * owner.Stats.GetStat(StatTag.AreaOfEffect), 1);
        dangerZoneSize = new Vector2(hit.GetComponent<BoxCollider2D>().size.x * owner.Stats.GetStat(StatTag.AreaOfEffect), hit.transform.localScale.y * owner.Stats.GetStat(StatTag.AreaOfEffect));
        owner.Stats.OnStatChanged[StatTag.AreaOfEffect].AddListener(OnAreaOfEffectChanges);
        cooldown = 0;
        skillCooldown = resultSkillCooldown;
        timeSetter.SetHitAnimationTime(skillCooldown / 10);
        damageModifier = new CombinedStat(100, 0, new List<float>());
        criticalStrikeChanceModifier.AddBaseValue(5);
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        //base.UseSkill();
        if (cooldown <= 0)
        {
            timeSetter.SetHitAnimationTime(skillCooldown / 10);
            attackHit.SetHit(getHitData());
            //AttackHit.SelfTransform.position = AttackHit.SelfTransform.position + (castPoint - selfTransform.position); //Ограничить по attackRange
            Vector3 castPosition = Vector3.ClampMagnitude(Vector3.Normalize(castPoint - selfTransform.position), attackRange);
            attackHit.SelfTransform.position = selfTransform.position + castPosition;
            attackHit.SelfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - selfTransform.position.y, castPoint.x - selfTransform.position.x) * Mathf.Rad2Deg - 90)); //Переписать
            skillCooldown = skillCooldown * resultCooldownRecoverySpeed;

            DangerZone.SetActive(new Vector2(dangerZoneSize.x, Vector3.Magnitude(castPoint - transform.position)), skillCooldown / 3);
            DangerZone.SetRotation(Vector3.Normalize(castPoint - transform.position));

            cooldown = skillCooldown;
            attackCoroutine = attackTimeDelay();
            StartCoroutine(attackCoroutine);
            //cooldownCoroutine = cooldownRecovery(); ???
            //StartCoroutine(cooldownCoroutine);
            return true;
        }
        else
            return false;
    }

    public override void StopUseSkill()
    {
        base.StopUseSkill();
        attackHit.SetActiveHit(false);
        StopCoroutine(attackCoroutine);
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        attackHit.OnFeedbackReceived.RemoveListener(owner.TakeHitFeedback);
        owner.Stats.OnStatChanged[StatTag.AreaOfEffect].RemoveListener(OnAreaOfEffectChanges);
        StopCoroutine(attackCoroutine);
        Destroy(attackHit.gameObject);
    }

    private void OnAreaOfEffectChanges(float value)
    {
        attackHit.transform.localScale = new Vector3(attackHit.transform.localScale.x * owner.Stats.GetStat(StatTag.AreaOfEffect), attackHit.transform.localScale.y * owner.Stats.GetStat(StatTag.AreaOfEffect), 1);
    }

    private IEnumerator attackTimeDelay()
    {
        //DangerZone.SetActive(areaOfEffectModifier.Value, cooldown / 3);
        yield return new WaitForSeconds(skillCooldown / 3);
        attackHit.SetActiveHit(true);
        attackHit.SelfTransform.Translate(new Vector3(0, 0.00001f, 0));
        while (cooldown > 0)
        {
            yield return new WaitForEndOfFrame();
            cooldown -= Time.deltaTime;
        }
        attackHit.SetActiveHit(false);
    }

    //private IEnumerator cooldownRecovery()
    //{
    //    while(base.cooldown > 0)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        base.cooldown -= Time.deltaTime;
    //    }    
    //}
}
