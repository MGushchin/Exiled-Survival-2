using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Statuses;

public class DivineAura : Skill
{
    private class auraFxComponents
    {
        public GameObject FxObject;
        public DisappearingDelay Delay;
    }
    public GameObject StatusZonePrefab;
    public GameObject AuraFXPrefab;
    private Transform selfTransform;
    //Utility
    private IEnumerator cooldownCoroutine;
    private StatusZoneApplicator auraZone;
    private auraFxComponents auraVisualEffects;
    //Skill Params
    private float baseSkillCooldown = 20;
    private CombinedStat cooldownModifier = new CombinedStat(20, 0, new List<float>());
    //private float baseSkillRadius = 1;
    private CombinedStat radiusModifier = new CombinedStat(1, 0, new List<float>());
    private CombinedStat auraBuffValueModifier = new CombinedStat(10, 0, new List<float>());
    private CombinedStat auraStrongBuffValueModifier = new CombinedStat(20, 0, new List<float>());
    private CombinedStat strongAuraDuration = new CombinedStat(4, 0, new List<float>());
    //Utility Params
    private Status ApplyingStatus;
    private Status StrongApplyingStatus;
    private IEnumerator permanentAuraApplyCoroutine;
    private float permanentAuraSpamDelay = 0;
    #region UtilityLinks
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(cooldownModifier)));
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private float resultRadius => owner.Stats.GetAdvancedStat(StatTag.AreaOfEffect).ModValueWithAddedParams(radiusModifier);
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;
        //Init gameObject
        //AuraZone
        auraZone = Instantiate(StatusZonePrefab).GetComponent<StatusZoneApplicator>();
        auraZone.transform.parent = selfTransform.transform;
        auraZone.transform.localPosition = Vector3.zero;
        auraZone.gameObject.transform.localScale = new Vector3(resultRadius, resultRadius, 1);
        auraZone.SetTargetAlly(owner.Ally);
        //Fx
        auraVisualEffects = new auraFxComponents();
        auraVisualEffects.FxObject = Instantiate(AuraFXPrefab);
        auraVisualEffects.Delay = auraVisualEffects.FxObject.GetComponent<DisappearingDelay>();
        auraVisualEffects.FxObject.transform.parent = selfTransform.transform;
        auraVisualEffects.FxObject.transform.localPosition = Vector3.zero;
        auraVisualEffects.FxObject.SetActive(false);
        //Init aura effect status
        ApplyingStatus = new Status(StatusType.DivineAura, 1, (int)auraBuffValueModifier.Value);
        ApplyingStatus.Buffs.Add(new Buff() { Tag = StatTag.damage, Type = StatModType.More, Value = auraBuffValueModifier.Value});
        StrongApplyingStatus = new Status(StatusType.DivineAura, 4, (int)auraStrongBuffValueModifier.Value);
        StrongApplyingStatus.Buffs.Add(new Buff() { Tag = StatTag.damage, Type = StatModType.More, Value = auraStrongBuffValueModifier.Value });
        //Stat changes events
        owner.Stats.OnStatChanged[StatTag.AreaOfEffect].AddListener(UpdateRadius);
        //owner.Stats.OnStatChanged[StatTag.Duration].AddListener(UpdateRadius);
        auraZone.AddUnitAsEntered(owner);
        permanentAuraApplyCoroutine = permanentAuraApplier();
        StartCoroutine(permanentAuraApplyCoroutine);
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            permanentAuraSpamDelay = strongAuraDuration.Value;
            auraZone.ApplyStatus(StrongApplyingStatus.GetCopy(), owner);
            auraVisualEffects.Delay.SetDelay(strongAuraDuration.Value);
            auraVisualEffects.FxObject.SetActive(true);
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

    private IEnumerator permanentAuraApplier()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            auraZone.ApplyStatus(ApplyingStatus.GetCopy(), owner);
            yield return new WaitForSeconds(1 + permanentAuraSpamDelay);
            permanentAuraSpamDelay = 0;
        }
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
            case ("Divine Aura Effect"):
                {
                    auraBuffValueModifier.AddIncreaseValue(10);
                    auraStrongBuffValueModifier.AddIncreaseValue(10);
                }
                break;
            case ("Divine Aura Duration"):
                {
                    strongAuraDuration.AddIncreaseValue(10);
                }
                break;
            case ("Divine Aura Cooldown"):
                {
                    cooldownModifier.AddIncreaseValue(10);
                }
                break;
            default:
                {

                }
                break;
        }
    }

    public void UpdateRadius(float value)
    {
        auraZone.transform.localScale = new Vector3(resultRadius, resultRadius, 1);
    }
}
