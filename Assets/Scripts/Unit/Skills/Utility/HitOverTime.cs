using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitOverTime : Hit
{
    [SerializeField]
    private float damageInterval = 1;
    private IEnumerator timer;
    private List<UnitActions> enteredUnits = new List<UnitActions>();

    public void SetDamageInterval(float value)
    {
        damageInterval = value;
    }

    public override void SetActiveHit(bool value)
    {
        base.SetActiveHit(value);
        timer = intervalDamageDealer();
        StartCoroutine(timer);
    }

    private void OnDisable()
    {
        enteredUnits.Clear();
    }

    private IEnumerator intervalDamageDealer()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(damageInterval);
            for(int i=0; i < enteredUnits.Count; i++)
            {
                HitFeedback feedback;
                feedback = enteredUnits[i].TakeDamage(data);
                OnFeedbackReceived.Invoke(feedback);
                OnHit.Invoke(SelfTransform.position);
                if (feedback.KillingBlow)
                    i--;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<UnitActions>() != null)
        {
            UnitActions unit = other.GetComponent<UnitActions>();
            if (unit.Ally != data.Ally)
            {
                enteredUnits.Add(unit);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<UnitActions>() != null)
        {
            UnitActions unit = collision.GetComponent<UnitActions>();
            if (unit.Ally != data.Ally)
            {
                enteredUnits.Remove(unit);
            }
        }
    }
}
