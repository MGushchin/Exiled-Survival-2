using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackDashing : Skill
{
    public GameObject TrailPrefab;
    public GameObject DangerZonePrefab;
    public GameObject HitZonePrefab;
    private Transform selfTransform;
    //Utility
    private Rigidbody2D ownerRb;
    private IEnumerator cooldownCoroutine;
    private TrailRenderer trail;
    private VectorDangerZoneRenderer DangerZone;
    private Hit dashingHit;
    //Skill Params
    private float baseSkillCooldown = 2;
    private CombinedStat cooldownModifier = new CombinedStat(1, 0, new List<float>());
    private float channelingTime = 1;

    //Utility Params
    private float dashTime = 0.25f;
    private float dashForce = 500; //162 ~ 1 unit

    #region UtilityLinks
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(cooldownModifier)));
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;
        ownerRb = owner.gameObject.GetComponent<Rigidbody2D>();
        //Init gameObject
        trail = Instantiate(TrailPrefab).GetComponent<TrailRenderer>();
        trail.transform.parent = selfTransform;
        trail.transform.localPosition = Vector3.zero;
        trail.enabled = false;
        DangerZone = Instantiate(DangerZonePrefab, transform).GetComponent<VectorDangerZoneRenderer>();
        dashingHit = Instantiate(HitZonePrefab, transform).GetComponent<Hit>();
        dashingHit.SetActiveHit(false);
        //Stat changes events

    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            //channeling = true;
            trail.enabled = true;
            HitData hit = getHitData();
            dashingHit.SetHit(hit);
            StartCoroutine(dashing(castPoint));
            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(baseSkillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);
            return true;
        }
        else
            return false;
    }

    public override void StopUseSkill()
    {
        base.StopUseSkill();
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        StopCoroutine(cooldownCoroutine);
        //Removing events links
    }

    private IEnumerator dashing(Vector3 castPoint)
    {
        owner.Movement.SetMove(false);
        DangerZone.SetActive(new Vector2(1, 3), channelingTime); 
        //DangerZone.transform.rotation = Quaternion.Euler(DangerZone.transform.rotation.eulerAngles.x, DangerZone.transform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - DangerZone.transform.position.y, castPoint.x - DangerZone.transform.position.x) * Mathf.Rad2Deg + 90)); //Переписать
        //yield return new WaitForSeconds(channelingTime);
        float channelingTimer = channelingTime;
        while (channelingTimer > 0)
        {
            DangerZone.SetRotation(Vector3.Normalize(castPoint - transform.position)); /*Quaternion.Euler(DangerZone.transform.rotation.eulerAngles.x, DangerZone.transform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - DangerZone.transform.position.y, castPoint.x - DangerZone.transform.position.x) * Mathf.Rad2Deg)); //Переписать*/
            //DangerZone.SetRotation(Vector3.Normalize(owner.SkillsActivation.CastPosition - transform.position));
            channelingTimer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        float dashTimer = dashTime;
        Vector3 startPos = transform.position;

        dashingHit.SetActiveHit(true);
        while (dashTimer > 0)
        {
            Vector3 force = Vector3.Normalize(castPoint - transform.position);
            force += new Vector3(0.0001f, 0.0001f, 0);
            ownerRb.AddForce(force * dashForce * Mathf.Clamp(dashTimer, 0, 1), ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
            dashTimer -= Time.fixedDeltaTime;
        }
        /*Debug.Log("Attack dash distance = " + Vector3.Magnitude(transform.position - startPos)); *///Разные дальности пофиксить
        dashingHit.SetActiveHit(false);
        owner.Movement.SetMove(true);
        ownerRb.velocity = new Vector2(0, 0);
        trail.enabled = false;
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
        switch (mod.name)
        {
            case (""):
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
