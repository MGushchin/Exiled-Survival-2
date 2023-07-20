using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Statuses;

[System.Serializable]
public class HitData
{
    public HitData(UnitActions hitSender)
    {
        HitSender = hitSender;
        PhysicalDamage = 0;
        CriticalStrikeChance = 0;
        CriticalStrikeMultiplier = 0;
        FireDamage = 0;
        ColdDamage = 0;
        LightningDamage = 0;
        Ally = false;
    }
    public UnitActions HitSender;
    public float PhysicalDamage;
    public float CriticalStrikeChance; //Возможно переписать
    public float CriticalStrikeMultiplier;
    public float FireDamage;
    public float ColdDamage;
    public float LightningDamage;
    public bool Ally;
    public List<Status> InflicktedStatuses = new List<Status>();
    public List<SkillTag> Tags = new List<SkillTag>();
}

[System.Serializable]
public class DotHitData
{
    public DotHitData()
    {
        Damage = 0;
        KillingBlow = false;
    }
    public float Damage;
    public bool KillingBlow;
}

public class HitFeedback
{
    public HitFeedback()
    {
        KillingBlow = false;
        IsCritical = false;
        DamageDealth = 0;
    }
    public bool KillingBlow;
    public bool IsCritical;
    public float DamageDealth;
    public UnitActions hitTaker;
    public Vector3 hitPosition;
}

public class Hit : MonoBehaviour
{
    public Transform SelfTransform; //Возможно перенос
    public Collider2D HitCollider;
    public UnityEvent<HitFeedback> OnFeedbackReceived = new UnityEvent<HitFeedback>();
    public UnityEvent<Vector3> OnHit = new UnityEvent<Vector3>();
    protected HitData data;

    public virtual void SetActiveHit(bool value)
    {
        gameObject.SetActive(value);
    }
    public void SetActiveHitAtPosition(bool value, Vector3 position)
    {
        SelfTransform.position = position;
        gameObject.SetActive(value);
    }

    public void SetActiveHitAtPosition(Vector3 position)
    {
        SelfTransform.position = position;
        gameObject.SetActive(true);
    }

    public void SetHit(HitData data)
    {
        this.data = data;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<UnitActions>() != null)
        {
            UnitActions unit = other.GetComponent<UnitActions>();
            if(unit.Ally != data.Ally)
            {
                HitFeedback feedback;
                Vector3 collisionPoint = other.ClosestPoint(transform.position);
                feedback = unit.TakeDamage(data, collisionPoint);
                OnFeedbackReceived.Invoke(feedback);
                OnHit.Invoke(SelfTransform.position);
            }
        }
    }
}
