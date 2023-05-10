using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Statuses
{
    public enum StatusType
    {
        Poison,
        DivineAura,
        Bleeding
    }

    [System.Serializable]
    public struct Buff
    {
        public StatTag Tag;
        public StatModType Type;
        public float Value;
    }

    [System.Serializable]
    public class Status
    {
        //Main section
        private StatusType type;
        public StatusType Type => type;
        //private UnitActions inflicter;
        //public UnitActions Inflicter => inflicter;
        //private int maximumStacks;
        //public int MaximumStacks => maximumStacks;
        private float duration;
        public float Duration => duration;
        [SerializeField]
        private float remainingDuration;
        public float RemainingDuration => remainingDuration;
        private int priority;
        public int Priority => priority;

        //public Status()
        //{

        //}

        public Status(StatusType _type, float _duration, int _priority)
        {
            type = _type;
            //inflicter = _inflicter;
            //maximumStacks = _maximumStacks;
            duration = _duration;
            remainingDuration = _duration;
            priority = _priority;
            damageTags = new List<StatTag>();
            damage = 0;
            Buffs = new List<Buff>();
        }

        public Status GetCopy()
        {
            Status copy = new Status(type/*, inflicter*//*, maximumStacks*/, duration, priority);
            foreach(StatTag tag in damageTags)
            {
                copy.damageTags.Add(tag);
            }
            copy.damage = damage;
            foreach (Buff buff in Buffs)
            {
                copy.Buffs.Add(buff);
            }
            return copy;
        }

        ///<summary>
        ///Return true if remaining duration <= 0.
        ///</summary>
        public bool Tick(float tickTime)
        {
            if (RemainingDuration - tickTime <= 0)
            {
                damage *= RemainingDuration;
                remainingDuration -= tickTime;
                return true;
            }
            else
            {
                remainingDuration -= tickTime;
                return false;
            }
        }
        //Damage section
        public List<StatTag> damageTags;
        public float damage;
        //Debuff section
        public List<Buff> Buffs;
    }
}
