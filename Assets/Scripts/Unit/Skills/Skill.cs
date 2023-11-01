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
        
    }

    protected virtual HitData getHitData() //Под рефакторинг
    {
        #region StatsMethod
        Debug.Log("Getted hit data");
        HitData hit = new HitData(owner);
        hit.Ally = owner.Ally;

        hit.Damage = owner.Stats.GetDamageData();

        foreach (CombinedStat damageType in hit.Damage.Values)
        {
            Debug.Log("Damage 0 " + damageType.Value);
        }

        foreach (StatTag tag in hitTags)
            foreach (CombinedStat damageType in hit.Damage.Values)
            {
                Debug.Log("Damage 1 " + tag + " "+ damageType.Value);
                damageType.ValueWithAddedModParams(owner.Stats.GetAdvancedStat(tag));
                Debug.Log("Damage 2 " + tag + " " + damageType.Value);
            }

        hit.CriticalStrikeChance = owner.Stats.GetAdvancedStat(StatTag.CriticalStrikeChance).ValueWithAddedParams(criticalStrikeChanceModifier); //Переписать
        hit.CriticalStrikeMultiplier = criticalStrikeMultiplierModifier.ValueWithAddedParams(owner.Stats.GetAdvancedStat(StatTag.CriticalStrikeMultiplier));

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

        foreach (CombinedStat damageType in hit.Damage.Values)
        {
            Debug.Log("DamageModifierValue " + damageModifier.Value + " = " + (damageModifier.Value - 100));
            damageType.AddMoreValue(damageModifier.Value - 100);
            Debug.Log("Damage i " + damageType.Value);
        }

        foreach (Status status in hit.InflicktedStatuses)
        {
            status.damage *= damageModifier.Value / 100;
        }

        foreach(CombinedStat damage in hit.Damage.Values)
        {
            hit.DamageDebug.Add(damage);
        }
        #endregion

        hit.Tags = skillTags;
        return hit;
    }

}
