using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Statuses;
using System;
using UnityEngine.Events;

public class StatusStorage : MonoBehaviour
{
    private class statusWithSender
    {
        public Status storedStatus;
        public UnitActions sender;

        public statusWithSender(Status status, UnitActions sender)
        {
            storedStatus = status;
            this.sender = sender;
        }
    }
    public UnityEvent<List<StatusType>, List<string>> OnStatusUpdate = new UnityEvent<List<StatusType>, List<string>>();
    [System.Serializable]
    private class statusExecuter
    {
        //public List<Status> statuses = new List<Status>();
        public List<statusWithSender> allStatuses = new List<statusWithSender>();
        public int activeStatusesCount = 0;
        public string StatusDescription
        {
            get
            {
                if (allStatuses.Count > 0)
                {
                    if (allStatuses.Count == 1 || maximumStacks == 1)
                        return allStatuses[0].storedStatus.RemainingDuration.ToString();
                    else
                        return activeStatusesCount.ToString();
                }
                else
                    return "";
            }
        }

        private int maximumStacks = 999;
        public int MaximumStacks => maximumStacks;
        private UnitActions owner;

        public statusExecuter(int maximumStacks, UnitActions owner)
        {
            this.maximumStacks = maximumStacks;
            this.owner = owner;
        }

        public void AddStatus(Status status, UnitActions sender)
        {
            statusWithSender expandedStatus = new statusWithSender(status, sender);
            allStatuses.Add(expandedStatus);
            activeStatusesCount = Mathf.Clamp(activeStatusesCount + 1, 0, maximumStacks);
            if (allStatuses.Count <= maximumStacks)
            {
                foreach (Buff buff in status.Buffs)
                {
                    owner.Stats.AddStat(buff.Tag, buff.Type, buff.Value, status.Type.ToString());
                }
            }
            else
            {
                recalculateActiveStatuses(); //Под рефакторинг
            }
        }

        public List<statusWithSender> ExecuteDamageEffects(float deltaTime)
        {
            List<statusWithSender> statusesToRemove = new List<statusWithSender>();
            if (allStatuses.Count > 0)
            {
                float damageMod = owner.Stats.GetTagMods(allStatuses[0].storedStatus.damageTags);
                int lastIndex = 0;
                for (int i = 0; i < activeStatusesCount; i++, lastIndex++)
                {
                    if (allStatuses[i].storedStatus.Tick(deltaTime))
                    {
                        statusesToRemove.Add(allStatuses[i]);
                    }
                    DotHitData data = new DotHitData();
                    data.Damage = allStatuses[i].storedStatus.damage * damageMod * deltaTime;
                    HitFeedback feedback = owner.TakeDotDamage(data);
                    if (feedback.KillingBlow) //Переработать прерывание
                        return allStatuses;
                    allStatuses[i].sender.TakeDotFeedback(feedback);
                }
                for (int i = activeStatusesCount; i < allStatuses.Count; i++) // Переписать это
                {
                    if (allStatuses[i].storedStatus.Tick(deltaTime))
                    {
                        statusesToRemove.Add(allStatuses[i]);
                    }
                }
            }
            return statusesToRemove;
        }

        public void RemoveStatus(statusWithSender status)
        {
            allStatuses.Remove(status);
            activeStatusesCount--;
            int statusIndex = allStatuses.IndexOf(status);
            if (statusIndex == -1)
                Debug.LogError("Status index = -1");
            if (statusIndex < maximumStacks)
            {
                foreach (Buff buff in status.storedStatus.Buffs)
                {
                    owner.Stats.RemoveStat(buff.Tag, buff.Type, buff.Value, status.storedStatus.Type.ToString());
                }
                recalculateActiveStatuses();
            }
            fillEmptyActiveStatuses();
        }

        public void RemoveStatuses(List<statusWithSender> statuses)
        {
            int removedActiveStatuses = 0;
            foreach (statusWithSender status in statuses)
            {
                int statusIndex = allStatuses.IndexOf(status);
                if (statusIndex == -1)
                    Debug.LogError("Status index = -1");
                else if (statusIndex < maximumStacks)
                {
                    foreach (Buff buff in status.storedStatus.Buffs)
                    {
                        owner.Stats.RemoveStat(buff.Tag, buff.Type, buff.Value, status.storedStatus.Type.ToString());
                    }
                    removedActiveStatuses++;
                }

            }
            for (int i = 0; i < statuses.Count; i++) //Переписать
            {
                //activeStatusesCount--;
                allStatuses.Remove(statuses[i]);
            }
            activeStatusesCount -= removedActiveStatuses;
            fillEmptyActiveStatuses();
            recalculateActiveStatuses();
        }

        public void Clear()
        {
            RemoveStatuses(allStatuses);
        }
        //private void sort()
        //{
        //    Status temp;
        //    for (int i = 0; i < allStatuses.Count; i++)
        //    {
        //        for (int j = i + 1; j < allStatuses.Count; j++)
        //        {
        //            if (allStatuses[i].Priority > allStatuses[j].Priority)
        //            {
        //                temp = allStatuses[i];
        //                allStatuses[i] = allStatuses[j];
        //                allStatuses[j] = temp;
        //            }
        //        }
        //    }
        //}

        private void fillEmptyActiveStatuses() // Под рефакторинг
        {
            int startI = activeStatusesCount;
            for(int i= startI; i < maximumStacks && i < allStatuses.Count; i++)
            {
                    Debug.Log("allStatuses.Count < maximumStacks");
                    foreach (Buff buff in allStatuses[i].storedStatus.Buffs)
                    {
                        owner.Stats.AddStat(buff.Tag, buff.Type, buff.Value, allStatuses[i].storedStatus.Type.ToString());
                    }
                activeStatusesCount++;
            }
        }

        private void sort()
        {
            statusWithSender temp;
            for (int i = 0; i < allStatuses.Count; i++)
            {
                for (int j = i + 1; j < allStatuses.Count - 1; j++)
                {
                    if (allStatuses[j].storedStatus.Priority > allStatuses[j + 1].storedStatus.Priority)
                    {
                        temp = allStatuses[j + 1];
                        allStatuses[j + 1] = allStatuses[j];
                        allStatuses[j] = temp;
                    }
                }
            }
        }

        private void recalculateActiveStatuses() //Переписать
        {
            for (int i = 0; i < maximumStacks && i < allStatuses.Count; i++) //Очень грязно, переписать
            {
                foreach (Buff buff in allStatuses[i].storedStatus.Buffs)
                {
                    owner.Stats.RemoveStat(buff.Tag, buff.Type, buff.Value, allStatuses[i].storedStatus.Type.ToString());
                }
            }
            sort();
            for (int i = 0; i < maximumStacks && i < allStatuses.Count; i++) //Очень грязно, переписать
            {
                foreach (Buff buff in allStatuses[i].storedStatus.Buffs)
                {
                    owner.Stats.AddStat(buff.Tag, buff.Type, buff.Value, allStatuses[i].storedStatus.Type.ToString());
                }
            }
        }
    }

    private UnitActions owner;
    private bool active = false;
    private IEnumerator updateCoroutine;
    //private float updateTime = 0.25f;
    private Dictionary<StatusType, statusExecuter> activeStatuses = new Dictionary<StatusType, statusExecuter>();
    [SerializeField]
    private List<Status> poisonStatus = new List<Status>(); //Debug
    [SerializeField]
    private List<Status> divineAuraStatus = new List<Status>(); //Debug

    public void Init(UnitActions owner)
    {
        this.owner = owner;
        foreach (StatusType value in Enum.GetValues(typeof(StatusType)))
        {
            int maxStacks;
            switch (value)
            {
                case (StatusType.Poison):
                    {
                        maxStacks = 999;
                    }
                    break;
                case (StatusType.DivineAura):
                    {
                        maxStacks = 1;
                    }
                    break;
                case (StatusType.Bleeding):
                    {
                        maxStacks = 999;
                    }
                    break;
                default:
                    {
                        Debug.LogWarning("Default exception");
                        maxStacks = 999;
                    }break;
            } //Определение максимума стаков для каждого эффекта
            //statusExecuter execute = activeStatuses[value] = new statusExecuter(maxStacks, owner, value);
            activeStatuses.Add(value, new statusExecuter(maxStacks, owner));
        }
    }

    public void SetActive(bool value)
    {
        active = value;
        if (active)
        {
            //updateCoroutine = update(updateTime);
            //StartCoroutine(updateCoroutine);
        }
        else
        {
            //StopCoroutine(updateCoroutine);
        }
    }

    private void Update() //Переместить в корутину, но в ней пока не работает
    {
        List<statusWithSender> statusesToRemove = new List<statusWithSender>();
        foreach (statusExecuter executer in activeStatuses.Values)
        {
            statusesToRemove = executer.ExecuteDamageEffects(Time.deltaTime);
            removeStatuses(statusesToRemove);
        }
    }

    private IEnumerator update(float deltaTime)
    {
        while (active)
        {
            yield return new WaitForSeconds(deltaTime);
        }
    }

    public void AddStatus(Status status, UnitActions sender)
    {
        activeStatuses[status.Type].AddStatus(status, sender);
        switch (status.Type)
        {
            case (StatusType.Poison):
                {
                    poisonStatus.Add(status);
                }break;
            case (StatusType.DivineAura):
                {
                    divineAuraStatus.Add(status);
                }
                break;
        } //Debug observer
        InvokeStatusChanges();
    }

    //public void RemoveStatus(Status status)
    //{
    //    activeStatuses[status.Type].RemoveStatus(status);
    //    switch (status.Type)
    //    {
    //        case (StatusType.Poison):
    //            {
    //                poisonStatus.Remove(status);
    //            }
    //            break;
    //        case (StatusType.DivineAura):
    //            {
    //                divineAuraStatus.Remove(status);
    //            }
    //            break;
    //    } //Debug observer
    //    InvokeStatusChanges();
    //}

    private void removeStatuses(List<statusWithSender> removeStatuses)
    {
        if (removeStatuses.Count > 0)
        {
            for (int i = 0; i < removeStatuses.Count; i++)
            {
                switch (removeStatuses[i].storedStatus.Type)
                {
                    case (StatusType.Poison):
                        {
                            poisonStatus.Remove(removeStatuses[i].storedStatus);
                        }
                        break;
                    case (StatusType.DivineAura):
                        {
                            divineAuraStatus.Remove(removeStatuses[i].storedStatus);
                        }
                        break;
                } //Debug observer
            }
            activeStatuses[removeStatuses[0].storedStatus.Type].RemoveStatuses(removeStatuses);
            removeStatuses.Clear();
            InvokeStatusChanges();
        }
    }

    public List<Status> GetStatusInfo(StatusType statusType)
    {
        List<Status> active = new List<Status>();
        for (int i = 0; i < activeStatuses[statusType].allStatuses.Count && i < activeStatuses[statusType].MaximumStacks; i++)
        {
            active.Add(activeStatuses[statusType].allStatuses[i].storedStatus);
        }
        return active;
    }

    public void InvokeStatusChanges()
    {
        List<StatusType> tags = new List<StatusType>();
        List<string> descriptions = new List<string>();
        foreach(statusExecuter executer in activeStatuses.Values)
        {
            if (executer.allStatuses.Count > 0)
            {
                tags.Add(executer.allStatuses[0].storedStatus.Type);
                descriptions.Add(executer.StatusDescription);
            }
        }
        OnStatusUpdate.Invoke(tags, descriptions);
    }

    public void Clear()
    {
        foreach (statusExecuter executer in activeStatuses.Values)
        {
            removeStatuses(executer.allStatuses);
            executer.activeStatusesCount = 0; //Перевести во внутреннюю логику
            if(executer.activeStatusesCount > 0 || executer.allStatuses.Count > 0)
                Debug.LogError("Executer not cleared active statuses " + executer.activeStatusesCount +" or all statuses " + executer.allStatuses.Count);
        }
    }
}
