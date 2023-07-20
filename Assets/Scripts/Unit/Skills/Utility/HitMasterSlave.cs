using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMasterSlave : Hit
{
    private HitMasterSlave master;
    private List<UnitActions> collidedUnits = new List<UnitActions>();

    public void SetMaster(HitMasterSlave master)
    {
        this.master = master;
    }

    public bool CheckContainUnit(UnitActions unit)
    {
        if (collidedUnits.Contains(unit))
            return true;
        else
        {
            collidedUnits.Add(unit);
            return false;
        }
    }

    private void OnEnable()
    {
        collidedUnits.Clear();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<UnitActions>() != null)
        {
            UnitActions unit = other.GetComponent<UnitActions>();
            if(!master.CheckContainUnit(unit))
                if (unit.Ally != data.Ally)
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
