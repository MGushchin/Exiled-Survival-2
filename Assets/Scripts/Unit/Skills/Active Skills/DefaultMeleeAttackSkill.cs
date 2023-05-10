using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMeleeAttackSkill : Skill
{
    public GameObject HitPrefab;
    private Hit attackHit;
    private Transform selfTransform;
    //Utility
    private IEnumerator attackCoroutine;
    //private IEnumerator cooldownCoroutine;
    //Skill params
    //private float skillCooldown = 1;
    private float baseSkillCooldown = 1;
    private float attackRange = 1;
    private float baseCriticalStrikeChance = 5;
    #region UtilityLinks
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value)); // Добавить модификатор скорости атаки
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        base.InitSKill(skillOwner);
        selfTransform = gameObject.transform;
        GameObject hit = Instantiate(HitPrefab);
        attackHit = hit.GetComponent<Hit>();
        attackHit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
        attackHit.SetActiveHit(false);
        cooldown = 0;
        skillCooldown = resultSkillCooldown;
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        //base.UseSkill();
        if (cooldown <= 0)
        {
            attackHit.SetHit(owner.Stats.GetHitData(baseCriticalStrikeChance));
            //AttackHit.SelfTransform.position = AttackHit.SelfTransform.position + (castPoint - selfTransform.position); //Ограничить по attackRange
            attackHit.SelfTransform.position = selfTransform.position + (Vector3.Normalize(castPoint - selfTransform.position) * attackRange);
            attackHit.SelfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - selfTransform.position.y, castPoint.x - selfTransform.position.x) * Mathf.Rad2Deg - 90)); //Переписать
            skillCooldown = skillCooldown * resultCooldownRecoverySpeed;
            cooldown = skillCooldown;
            attackCoroutine = attackTimeDelay();
            StartCoroutine(attackCoroutine);
            //cooldownCoroutine = cooldownRecovery();
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
        StopCoroutine(attackCoroutine);
        Destroy(attackHit.gameObject);
    }

    private IEnumerator attackTimeDelay()
    {
        yield return new WaitForSeconds(cooldown / 2);
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
