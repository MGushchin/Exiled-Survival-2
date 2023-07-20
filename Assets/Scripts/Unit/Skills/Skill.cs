using Statuses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private SkillMod baseSkillMod;
    public SkillMod BaseSkillMod => baseSkillMod;
    public int Level => BaseSkillMod.Level;
    [SerializeField]
    private string skillName;
    public string Name => skillName;
    [SerializeField]
    private Sprite skillIcon;
    public Sprite SkillIcon => skillIcon;
    public Transform SelfTransform;
    [SerializeField]
    private bool activeSkill; //Возможно разделение на два дочерних класса активных и пассивных скиллов
    public bool ActiveSkill => activeSkill;
    protected float cooldown = 0;
    public float Cooldown => cooldown;
    protected float skillCooldown;
    public float SkillCooldown => skillCooldown;
    protected List<StatTag> hitTags = new List<StatTag>();
    [SerializeField]
    protected List<SkillTag> skillTags = new List<SkillTag>();
    public List<SkillTag> Tags => skillTags;

    protected UnitActions owner;

    #region SkillParams
    protected CombinedStat damageModifier = new CombinedStat(0, 0, new List<float>());
    protected CombinedStat attackSpeedModifier = new CombinedStat(0, 0, new List<float>());
    protected CombinedStat criticalStrikeChanceModifier = new CombinedStat(0, 0, new List<float>());
    protected CombinedStat criticalStrikeMultiplierModifier = new CombinedStat(0, 0, new List<float>());
    //protected float cooldownTime;
    //Ailments section

    //Utility Params
    #endregion

    #region UtilityLinks
    protected float attackRange => owner.Stats.GetStat(StatTag.AttackRange);
    protected float damageMultiplier => damageModifier.Value / 100;
    #endregion

    public void SetBaseSkillMod(SkillMod mod)
    {
        baseSkillMod = mod;
        foreach(SkillTag tag in mod.Tags)
            skillTags.Add(tag);
    }

    public virtual void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
    }


    public virtual bool UseSkill(Vector3 castPoint)
    {
        return true;
    }

    public virtual void StopUseSkill()
    {

    }

    public virtual void RemoveSkill()
    {
        
    }

    public void GetUpgrades()
    {

    }

    public virtual void ApplyUpgrade(SkillMod mod)
    {
        baseSkillMod.UpgradeLevel();
    }

    protected virtual HitData getHitData() //Под рефакторинг
    {
        //HitData hit = owner.Stats.GetHitData(hitTags);

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
        hit.PhysicalDamage = owner.Stats.GetAdvancedStat(StatTag.PhysicalDamage).ValueWithAddedParams(modStat.Value) * damageModifier.ModValue;//Переписать
        hit.FireDamage = owner.Stats.GetAdvancedStat(StatTag.FireDamage).ValueWithAddedParams(modStat.Value) * damageModifier.ModValue;//Переписать
        hit.ColdDamage = owner.Stats.GetAdvancedStat(StatTag.ColdDamage).ValueWithAddedParams(modStat.Value) * damageModifier.ModValue;//Переписать
        hit.LightningDamage = owner.Stats.GetAdvancedStat(StatTag.LightningDamage).ValueWithAddedParams(modStat.Value) * damageModifier.ModValue;//Переписать
        hit.CriticalStrikeChance = owner.Stats.GetAdvancedStat(StatTag.CriticalStrikeChance).ValueWithAddedParams(criticalStrikeChanceModifier); //Переписать
        hit.CriticalStrikeMultiplier = owner.Stats.GetAdvancedStat(StatTag.CriticalStrikeMultiplier).Value;
        if (hitTags.Contains(StatTag.PhysicalDamage) && owner.Stats.GetAdvancedStat(StatTag.BleedingChance).Value > 0) //Переписать
        {
            int garantedStacks = (int)(owner.Stats.GetAdvancedStat(StatTag.BleedingChance).Value / 100);
            int stacksCount = garantedStacks;
            if (Random.Range(0, 100) <= (owner.Stats.GetAdvancedStat(StatTag.BleedingChance).Value - garantedStacks * 100))
            {
                stacksCount++;
            }
            for (int i = 0; i < stacksCount; i++)
            {
                hit.InflicktedStatuses.Add(owner.Stats.GetStatus(StatusType.Bleeding)); //Ограничить по шансу и статусу
            }
        }
        #endregion

        hit.Tags = skillTags;
        //float criticalStrikeChance = owner.Stats.GetAdvancedStat(StatTag.CriticalStrikeChance).ValueWithAddedParams(criticalStrikeChanceModifier);
        //hit.CriticalStrikeChance = criticalStrikeChance; // Переписать
        //hit.PhysicalDamage *= damageModifier.ModValue;
        //hit.FireDamage *= damageModifier.ModValue;
        //hit.ColdDamage *= damageModifier.ModValue;
        //hit.LightningDamage *= damageModifier.ModValue;
        return hit;
    }

}
