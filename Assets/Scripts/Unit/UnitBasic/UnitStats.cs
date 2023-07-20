using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using Statuses;
using Random = UnityEngine.Random;
using System.Linq;

[System.Serializable]
public enum StatTag 
{ 
    life,
    lifeRegeneration,
    LifeLeech,
    damage,
    movementSpeed,
    Armour,
    Evasion,
    CooldownRecovery,
    PhysicalDamage,
    ElementalDamage,
    FireDamage,
    ColdDamage,
    LightningDamage,
    CriticalStrikeChance,
    CriticalStrikeMultiplier,
    ProjectileDamage,
    Pierce,
    ProjectileCount,
    AreaOfEffect,
    AreaDamage,
    PoisonChance,
    PoisonDamage,
    PoisonDuration,
    AttackDamage,
    SpellDamage,
    MultistrikeChance,
    BleedingChance,
    BleedingDamage,
    BleedingDuration,
    AttackRange,
    Duration
}

[System.Serializable]
public enum StatModType
{
    Base,
    Increase,
    More
}

[System.Serializable]
public struct StatMod
{
    [SerializeField]
    private StatTag tag;
    public StatTag Tag => tag;
    [SerializeField]
    private StatModType tagType;
    public StatModType TagType => tagType;
    [SerializeField]
    private float value;
    public float Value => value;
}

[System.Serializable]
public class SetterStatData
{
    public StatTag Tag;
    public float Base;
    public float Increase;
    public List<float> More = new List<float>();
}

public class UnitStats : MonoBehaviour
{
    [SerializeField]
    private UnitActions owner; // Возможно перенос
    public Dictionary<StatTag, UnityEvent<float>> OnStatChanged = new Dictionary<StatTag, UnityEvent<float>>(); // Продолжить, напистаь enum статов
    [HideInInspector]
    //public UnityEvent<HitData, List<StatTag>> OnPreparingHit = new UnityEvent<HitData, List<StatTag>>();
    private Dictionary<StatTag, CombinedStat> stats = new Dictionary<StatTag, CombinedStat>();
    [SerializeField]
    private bool ally;
    public bool Ally => ally;
    public List<SetterStatData> StartingStats = new List<SetterStatData>();
    [SerializeField]
    private List<effectDescription> effects = new List<effectDescription>();
    public Dictionary<StatTag, List<StatusType>> StatusesByDamageType = new Dictionary<StatTag, List<StatusType>>();
    [System.Serializable]
    private class effectDescription
    {
        public StatTag tag;
        public StatModType type;
        public float value;
        public string description;
        public string label;
        public effectDescription(StatTag tag, StatModType type, float value, string description)
        {
            this.tag = tag;
            this.type = type;
            this.value = value;
            this.description = description;
            label = $"{description}: {value} {type} {tag}";
        }

        public bool compare(StatTag tag, StatModType type, float value, string description)
        {
            string label = $"{description}: {value} {type} {tag}";
            if (label == this.label)
                return true;
            else
                return false;

        }
    }


    public void InitStats()
    {
        foreach (StatTag value in Enum.GetValues(typeof(StatTag))) //Возможно ограничить количество статов с эвентами
        {
            CombinedStat stat = new CombinedStat(0, 0, new List<float>());
            stats.Add(value, stat);
            OnStatChanged.Add(value, new UnityEvent<float>());
            stat.InitValue();
        }
        //Init Stat Section
        stats[StatTag.CooldownRecovery].AddBaseValue(1);
        stats[StatTag.CriticalStrikeChance].AddBaseValue(5);
        stats[StatTag.CriticalStrikeMultiplier].AddBaseValue(150);
        stats[StatTag.PoisonDamage].AddBaseValue(4);
        stats[StatTag.BleedingDamage].AddBaseValue(2);
        stats[StatTag.PoisonDuration].AddBaseValue(2);
        stats[StatTag.BleedingDuration].AddBaseValue(4);
        stats[StatTag.AttackRange].AddBaseValue(1);
        stats[StatTag.AreaOfEffect].AddBaseValue(1);
        //Statuses
        StatusesByDamageType.Add(StatTag.PhysicalDamage, new List<StatusType> { StatusType.Bleeding });
        //Debug

        foreach (SetterStatData stat in StartingStats)
        {
            stats[stat.Tag] = new CombinedStat(stat.Base, stat.Increase, stat.More);
        }
    }

    public HitData GetHitData() //Возможно перенос
    {
        HitData hit = new HitData(owner);
        hit.Ally = ally;
        hit.PhysicalDamage = stats[StatTag.PhysicalDamage].ValueWithAddedParams(stats[StatTag.damage]);
        hit.FireDamage = stats[StatTag.PhysicalDamage].ValueWithAddedParams(stats[StatTag.FireDamage]);
        hit.ColdDamage = stats[StatTag.PhysicalDamage].ValueWithAddedParams(stats[StatTag.ColdDamage]);
        hit.LightningDamage = stats[StatTag.PhysicalDamage].ValueWithAddedParams(stats[StatTag.LightningDamage]);
        hit.CriticalStrikeChance = stats[StatTag.CriticalStrikeChance].Value; //Переписать
        hit.CriticalStrikeMultiplier = stats[StatTag.CriticalStrikeMultiplier].Value;
        //OnPreparingHit.Invoke(hit, new List<StatTag>());
        return hit;
    }

    public Status GetStatus(StatusType type)
    {
        switch(type)
        {
            case (StatusType.Poison):
                {
                    Status status = new Status(StatusType.Poison, stats[StatTag.PoisonDuration].Value, (int)stats[StatTag.PoisonDamage].Value); //sender исправить, int преобразование
                    status.damage = stats[StatTag.PoisonDamage].Value;
                    return status;
                }
            case (StatusType.Bleeding):
                {
                    Status status = new Status(StatusType.Bleeding, stats[StatTag.BleedingDuration].Value, (int)stats[StatTag.BleedingDamage].Value); //sender исправить, int преобразование
                    status.damage = stats[StatTag.BleedingDamage].Value;
                    return status;
                }
        }
        //Status status = new Status(); // Debug
        Debug.LogError("Null status created");
        return null; //Переписать
    }

    public Status GetStatus(StatusType type, float magnitude, float duration)
    {
        switch (type)
        {
            case (StatusType.Poison):
                {
                    Status status = new Status(StatusType.Poison, duration * stats[StatTag.PoisonDuration].ModValue, (int)(magnitude * stats[StatTag.PoisonDamage].ModValue)); //sender исправить, int преобразование
                    status.damage = stats[StatTag.PoisonDamage].Value;
                    return status;
                }
            case (StatusType.Bleeding):
                {
                    Status status = new Status(StatusType.Bleeding, duration * stats[StatTag.BleedingDuration].ModValue, (int)(magnitude * stats[StatTag.BleedingDamage].Value)); //sender исправить, int преобразование
                    status.damage = stats[StatTag.BleedingDamage].Value;
                    return status;
                }
        }
        //Status status = new Status(); // Debug
        Debug.LogError("Null status created");
        return null; //Переписать
    }

    public HitData GetHitData(List<StatTag> tags) //Возможно перенос
    {
        HitData hit = new HitData(owner);
        hit.Ally = ally;
        CombinedModStat modStat = new CombinedModStat();
        foreach(StatTag tag in tags)
        {
            modStat.AddIncreaseValue(stats[tag].IncreaseValue);
            modStat.AddMoreValue(stats[tag].MoreValues);
        }
        hit.PhysicalDamage = stats[StatTag.PhysicalDamage].ValueWithAddedParams(stats[StatTag.damage]) * modStat.Value;//Переписать
        hit.FireDamage = stats[StatTag.FireDamage].Value * modStat.Value;//Переписать
        hit.ColdDamage = stats[StatTag.ColdDamage].Value * modStat.Value;//Переписать
        hit.LightningDamage = stats[StatTag.LightningDamage].Value * modStat.Value;//Переписать
        hit.CriticalStrikeChance = stats[StatTag.CriticalStrikeChance].Value; //Переписать
        hit.CriticalStrikeMultiplier = stats[StatTag.CriticalStrikeMultiplier].Value;
        if(tags.Contains(StatTag.PhysicalDamage) && stats[StatTag.BleedingChance].Value > 0) //Переписать
        {
            int garantedStacks = (int)(stats[StatTag.BleedingChance].Value / 100);
            int stacksCount = garantedStacks;
            if (Random.Range(0, 100) <= (stats[StatTag.BleedingChance].Value - garantedStacks * 100))
            {
                stacksCount++;
            }
            for(int i=0; i < stacksCount; i++)
            {
                hit.InflicktedStatuses.Add(GetStatus(StatusType.Bleeding)); //Ограничить по шансу и статусу
            }
        }
        //OnPreparingHit.Invoke(hit, tags);
        return hit;
    }

    
    public float GetTagMods(List<StatTag> tags) // Переписать
    {
        CombinedModStat modStat = new CombinedModStat();
        foreach (StatTag tag in tags)
        {
            modStat.AddIncreaseValue(stats[tag].IncreaseValue);
            modStat.AddMoreValue(stats[tag].MoreValues);
        }
        return modStat.ModValue;
    }

    public void SetStats(List<SetterStatData> startingStats)
    {
        StartingStats = startingStats;
        foreach (SetterStatData stat in startingStats)
        {
            stats[stat.Tag] = new CombinedStat(stat.Base, stat.Increase, stat.More);
            OnStatChanged[stat.Tag].Invoke(stats[stat.Tag].Value);
        }
    }

    public void AddStat(StatTag tag, StatModType type, float value)
    {
        CombinedStat stat = stats[tag];
        switch(type)
        {
            case (StatModType.Base):
                {
                    stat.AddBaseValue(value);
                }break;
            case (StatModType.Increase):
                {
                    stat.AddIncreaseValue(value);
                }
                break;
            case (StatModType.More):
                {
                    stat.AddMoreValue(value);
                }
                break;
            default:
                {
                    Debug.LogWarning("default exception");
                }break;
        }
        OnStatChanged[tag].Invoke(stat.Value);
    }

    public void AddStat(StatTag tag, StatModType type, float value, string description)
    {
        CombinedStat stat = stats[tag];
        effects.Add(new effectDescription(tag, type, value, description));
        switch (type)
        {
            case (StatModType.Base):
                {
                    stat.AddBaseValue(value);
                }
                break;
            case (StatModType.Increase):
                {
                    stat.AddIncreaseValue(value);
                }
                break;
            case (StatModType.More):
                {
                    stat.AddMoreValue(value);
                }
                break;
            default:
                {
                    Debug.LogWarning("default exception");
                }
                break;
        }
        OnStatChanged[tag].Invoke(stat.Value);
    }

    public void RemoveStat(StatTag tag, StatModType type, float value)
    {
        CombinedStat stat = stats[tag];
        switch (type)
        {
            case (StatModType.Base):
                {
                    stat.RemoveBaseValue(value);
                }
                break;
            case (StatModType.Increase):
                {
                    stat.RemoveIncreaseValue(value);
                }
                break;
            case (StatModType.More):
                {
                    stat.RemoveMoreValue(value);
                }
                break;
            default:
                {
                    Debug.LogWarning("default exception");
                }
                break;
        }
        OnStatChanged[tag].Invoke(stat.Value);
    }

    public void RemoveStat(StatTag tag, StatModType type, float value, string description)
    {
        CombinedStat stat = stats[tag];
        foreach(effectDescription effect in effects)
        {
            if (effect.compare(tag, type, value, description))
            {
                effects.Remove(effect);
                break;
            }
        }
        switch (type)
        {
            case (StatModType.Base):
                {
                    stat.RemoveBaseValue(value);
                }
                break;
            case (StatModType.Increase):
                {
                    stat.RemoveIncreaseValue(value);
                }
                break;
            case (StatModType.More):
                {
                    stat.RemoveMoreValue(value);
                }
                break;
            default:
                {
                    Debug.LogWarning("default exception");
                }
                break;
        }
        OnStatChanged[tag].Invoke(stat.Value);
    }

    public CombinedStat GetAdvancedStat(StatTag tag)
    {
        return stats[tag];
    }

    public float GetStat(StatTag tag)
    {
        return stats[tag].Value;
    }

    public float GetStatMod(StatTag tag)
    {
        return stats[tag].ModValue;
    }
}

