using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Statuses;

public class LifeFlask : Skill
{
    private class auraFxComponents
    {
        public GameObject FxObject;
        public DisappearingDelay Delay;
    }
    public GameObject FXPrefab;
    private Transform selfTransform;
    //Utility
    private IEnumerator cooldownCoroutine;
    private IEnumerator recoveryCoroutine;
    private auraFxComponents visualEffects;

    #region SkillParams
    private CombinedStat recoveryPerSecondModifier = new CombinedStat(8, 0, new List<float>());
    private CombinedStat durationRecoveryModifier = new CombinedStat(4, 0, new List<float>());
    private CombinedStat cooldownModifier = new CombinedStat(0, 0, new List<float>());
    private float baseSkillCooldown = 12;
    //Ailments section

    //Utility Params
    #endregion

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

        //Init methods
        //Fx
        visualEffects = new auraFxComponents();
        visualEffects.FxObject = Instantiate(FXPrefab);
        visualEffects.Delay = visualEffects.FxObject.GetComponent<DisappearingDelay>();
        visualEffects.FxObject.transform.parent = selfTransform.transform;
        visualEffects.FxObject.transform.localPosition = Vector3.zero;
        visualEffects.FxObject.SetActive(false);
        //Event subscriptions

    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            visualEffects.Delay.SetDelay(durationRecoveryModifier.Value);
            visualEffects.FxObject.SetActive(true);
            //Setup Hits
            recoveryCoroutine = recoveryOverTime();
            StartCoroutine(recoveryCoroutine);
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

    public override void StopUseSkill()
    {
        base.StopUseSkill();
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        StopCoroutine(recoveryCoroutine);
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

    private IEnumerator recoveryOverTime()
    {
        float remainingDuration = durationRecoveryModifier.Value;
        while (remainingDuration - Time.deltaTime > 0)
        {
            remainingDuration -= Time.deltaTime;
            owner.TakeHeal(recoveryPerSecondModifier.Value * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        owner.TakeHeal(recoveryPerSecondModifier.Value * remainingDuration);
    }

    public override void ApplyUpgrade(string name, int level)
    {
        //base.ApplyUpgrade(name, level);
        switch (name)
        {
            case ("Life Flask Recovery Per Second"):
                {
                    recoveryPerSecondModifier.AddBaseValue(1);
                }
                break;
            case ("Divine Aura Duration"):
                {
                    durationRecoveryModifier.AddBaseValue(1);
                }
                break;
            case ("Life Flask Cooldown"):
                {
                    recoveryPerSecondModifier.AddIncreaseValue(10);
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
