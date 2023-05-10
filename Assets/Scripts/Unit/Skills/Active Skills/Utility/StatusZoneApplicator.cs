using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Statuses;

public class StatusZoneApplicator : MonoBehaviour
{
    public Collider2D zoneCollider;
    //Pools
    [SerializeField]
    private List<UnitActions> enteredUnits = new List<UnitActions>();
    //Utility
    private bool targetAlly = false;

    public void SetTargetAlly(bool targetAlly)
    {
        this.targetAlly = targetAlly;
    }

    public void ApplyStatus(Status applyingStatus, UnitActions inflicter)
    {
        for(int i=0; i < enteredUnits.Count; i++)
        {
            enteredUnits[i].TakeStatus(applyingStatus, inflicter);
        }
    }

    public void RefreshTargets() //Debug
    {
        Clear();
        zoneCollider.enabled = false;
        zoneCollider.enabled = true;
    }

    public void AddUnitAsEntered(UnitActions unit)
    {
        if (unit.Ally == targetAlly)
            enteredUnits.Add(unit);
    }

    public void Clear()
    {
        enteredUnits.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<UnitActions>() != null)
        {
            UnitActions unit = collision.GetComponent<UnitActions>();
            if(unit.Ally == targetAlly)
                enteredUnits.Add(unit);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<UnitActions>() != null)
        {
            enteredUnits.Remove(collision.GetComponent<UnitActions>());
        }
    }
}
