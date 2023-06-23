using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Statuses;

public class Dash : Skill
{
    public GameObject TrailPrefab;
    private Transform selfTransform;
    //Utility
    private Rigidbody2D ownerRb;
    private IEnumerator cooldownCoroutine;
    private TrailRenderer trail;
    //Skill Params
    private float baseSkillCooldown = 4;
    private CombinedStat cooldownModifier = new CombinedStat(1, 0, new List<float>());
    private CombinedStat rangeModifier = new CombinedStat(1, 0, new List<float>());

    //Utility Params
    private float dashTime = 0.25f;
    private float dashForce = 200;

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
        //Stat changes events

    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            //Main execute
            //ownerRb.velocity = new Vector2(0, 0);
            //ownerRb.AddForce(owner.Movement.CurrentMoveDirection * dashForce);
            trail.enabled = true;
            StartCoroutine(dashing());
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

    private IEnumerator dashing()
    {
        float dashTimer = dashTime;
        while (dashTimer > 0)
        {
            Vector3 force = Vector3.Normalize(owner.Movement.CurrentMoveDirection);
            force += new Vector3(0.0001f, 0.0001f, 0);
            ownerRb.AddForce(force * dashForce * Mathf.Clamp(dashTimer, 0, 1), ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
            dashTimer -= Time.fixedDeltaTime;
        }
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
