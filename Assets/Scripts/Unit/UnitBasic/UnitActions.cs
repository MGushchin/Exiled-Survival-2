using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Statuses;


public class UnitActions : MonoBehaviour
{
    public bool Alive => alive;
    private bool alive = true;
    private bool stunned = false; //Перенос
    public Collider2D HitCollider; // Возможно перенести в другое место
    public Transform selfTransform;
    public UnitAnimationState Animations;
    public UnitMove Movement;
    public UnitStats Stats;
    public SkillActivator SkillsActivation;
    public StatusStorage Status;
    public UnitExperience Experience = new UnitExperience();
    public UnitDrop Drop;
    [HideInInspector]
    public UnityEvent<UnitActions> OnDeath = new UnityEvent<UnitActions>();
    [HideInInspector]
    public UnityEvent<int> OnLevelUp = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent<float, float> OnExperienceChanged = new UnityEvent<float, float>(); //перенести бары
    [HideInInspector]
    public UnityEvent<float, Vector3> OnTakingDamage = new UnityEvent<float, Vector3>();
    //Valuebars
    public Valuebar LifeValue;
    public bool Ally => Stats.Ally;
    private Vector3 currentPosition;
    private Vector3 currentDirection;

    //Private services
    private UnitRecovery recovery;

    //private bool dead = true; //Debug

    #region UnitState
    public int PoisonsInflicted => Status.GetStatusInfo(StatusType.Poison).Count;
    public bool FullLife => LifeValue.Value == LifeValue.MaximumValue;
    public bool LowLife => LifeValue.Value <= LifeValue.MaximumValue * 0.35f;
    #endregion


    //Utility
    private IEnumerator deathRattleCoroutine;
    private IEnumerator recoveringCoroutine;
    private IEnumerator stunningCoroutine;

    public void InitUnit()
    {
        #region Init Section
        Stats.InitStats();
        selfTransform = gameObject.transform;
        Movement.OnReachingPosition.AddListener(Stop);
        Movement.SetUnitMovespeed(Stats.GetAdvancedStat(StatTag.movementSpeed).Value);
        Stats.OnStatChanged[StatTag.movementSpeed].AddListener(Animations.SetMovementspeedMultiplier);
        Stats.OnStatChanged[StatTag.movementSpeed].AddListener(Movement.SetUnitMovespeed);
        Stats.OnStatChanged[StatTag.CooldownRecovery].AddListener(Animations.SetAttackSpeedMultiplier);
        Animations.SetAttackSpeedMultiplier(Stats.GetStat(StatTag.CooldownRecovery));
        Stats.OnStatChanged[StatTag.life].AddListener(LifeValue.AddMaximumValue);
        LifeValue.SetMaximumValue(Stats.GetStat(StatTag.life)); //Переписать аккуратнее
        LifeValue.AddValue(Stats.GetStat(StatTag.life)); //Переписать аккуратнее
        LifeValue.OnReachingZero.AddListener(Death);
        recovery = new UnitRecovery(this);
        Status.Init(this);
        SkillsActivation.InitSkills();
        recoveringCoroutine = recovering(); //Возможно перенос в Start()
        StartCoroutine(recoveringCoroutine); //Возможно перенос в Start()
        //Status.Init(this);
        Status.SetActive(true); //Возможно перенос в Start()
        #endregion
    }

    public void MoveToPosition(Vector3 position)
    {
        if (alive && !stunned)
        {
            Movement.MoveTo(position);
            Animations.SetState(UnitState.Run);
        }
    }

    public void Move(Vector3 moveOffset)
    {
        if (alive && !stunned)
        {
            if (moveOffset == Vector3.zero)
            {
                Stop();
            }
            else
            {
                Movement.Move(moveOffset);
                Animations.SetState(UnitState.Run);
            }
        }
    }

    public void LookInDirection(Vector3 direction) 
    {
        if (alive && !stunned)
        {
            Movement.LookAt(direction);
        }
        SkillsActivation.SetCastPoint(direction);
    }

    public void UseSkill(Vector3 direction, int skillIndex) 
    {
        if (alive && !stunned)
        {
            if(!Movement.IsMoving)
                Animations.SetState(UnitState.Attack);
            Movement.LookAt(direction);
            SkillsActivation.UseSkill(direction, skillIndex);
        }
    }

    public void Stop()
    {
        Movement.Stop();
        Animations.SetState(UnitState.Idle);
    }

    public HitFeedback TakeDamage(HitData hit)
    {
        HitFeedback feedback = new HitFeedback();
        float damage = hit.PhysicalDamage + hit.FireDamage + hit.ColdDamage + hit.LightningDamage;
        if (Random.Range(0, 100) <= hit.CriticalStrikeChance)
        {
            damage *= (hit.CriticalStrikeMultiplier / 100);
            feedback.IsCritical = true;
        }
        damage *= 1 - MechanicsCalc.CalculateDamageReduction(damage, Stats.GetStat(StatTag.Armour));
        foreach (Status status in hit.InflicktedStatuses)
            TakeStatus(status, hit.HitSender);
        float stunDuration = Mathf.Clamp(damage * 100 / Stats.GetStat(StatTag.life) / 100 / 2, 0, 1);
        if(stunDuration > 0.1)
        {
            stunningCoroutine = stunning(stunDuration);
            StartCoroutine(stunningCoroutine);
        }
        LifeValue.AddValue(-damage); //Минус
        feedback.DamageDealth = damage;
        if (LifeValue.Value == 0)
        {
            feedback.KillingBlow = true;
            Death();
        }

        OnTakingDamage.Invoke(damage, selfTransform.position);

        return feedback;
    }

    public HitFeedback TakeDotDamage(DotHitData dot)
    {
        HitFeedback feedback = new HitFeedback();
        float damage = dot.Damage;
        LifeValue.AddValue(-damage); // Не корректная передача через минус
        feedback.DamageDealth = damage;
        if (LifeValue.Value == 0) //Переписать мб
        {
            feedback.KillingBlow = true;
            Death();
        }

        OnTakingDamage.Invoke(damage, selfTransform.position);

        return feedback;
    }

    public void TakeHeal(float amount)
    {
        LifeValue.AddValue(amount);
    }

    public void TakeStatus(Status status, UnitActions inflicter)
    {
        Status.AddStatus(status, inflicter);
    }

    public void TakeHitFeedback(HitFeedback feedback)
    {
        //Debug.Log(gameObject.name + ": Received hit feedback " + feedback.DamageDealth + " " + feedback.KillingBlow);
        float vampirismInstance = feedback.DamageDealth * Stats.GetStat(StatTag.LifeLeech) / 100;
        recovery.AddVampirismInstance(vampirismInstance);
    }

    public void TakeDotFeedback(HitFeedback feedback)
    {
        //if(feedback.KillingBlow)
        //    Debug.Log(gameObject.name + ": Received dot feedback " + feedback.DamageDealth + " " + feedback.KillingBlow);
    }

    public void TakeExperience(float experience)
    {
        bool levelUp = Experience.AddExperience(experience); //Переработать
        OnExperienceChanged.Invoke(Experience.CurrentExperience, Experience.MaxiumExperience);
        if (levelUp)
            OnLevelUp.Invoke(Experience.Level);
    }

    public void Death()
    {
        if (alive) //Костыль
        {
            alive = false;
            stunned = false;
            Status.Clear();
            Stop();
            Animations.SetState(UnitState.Death);
            UnitPool.instance.RemoveFromPool(this, Stats.Ally);
            HitCollider.enabled = false;
            StopCoroutine(recoveringCoroutine);
            deathRattleCoroutine = deathRattle();
            StartCoroutine(deathRattleCoroutine);
            Drop.DropItems();
            //DropableItemsFactory.Instance.CreateExperienceItem(1, selfTransform.position); //Переписать
        }
    }

    public void Ressurect()
    {
        LifeValue.SetMaximumValue(Stats.GetStat(StatTag.life));
        LifeValue.FillToMaximum();
        alive = true;
        stunned = false;
        Animations.SetState(UnitState.Idle);
        HitCollider.enabled = true;
    }

    private IEnumerator deathRattle()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        OnDeath.Invoke(this);
    }

    private IEnumerator stunning(float duration)
    {
        stunned = true;
        Movement.Stop();
        Animations.SetState(UnitState.Stun);
        yield return new WaitForSeconds(duration);
        stunned = false;
    }

    private IEnumerator recovering()
    {
        float recoveryFrequency = 0.1f;
        while(alive)
        {
            yield return new WaitForSeconds(recoveryFrequency);
            //LifeValue.AddValue(Stats.GetStat(StatTag.lifeRegeneration) / 100 * recoveryFrequency * Stats.GetStat(StatTag.life));
            recovery.Update(recoveryFrequency);
        }
    }
}
