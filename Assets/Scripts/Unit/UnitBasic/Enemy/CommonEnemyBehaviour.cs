using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonEnemyBehaviour : MonoBehaviour
{

    public UnitActions Actions;
    [SerializeField]
    private float updateTime = 1;
    [SerializeField]
    private float attackRange = 1;
    private IEnumerator fighting;
    [SerializeField]
    private bool active = false;
    public bool ActiveSelf => active;
    private UnitActions currentTarget;
    private Transform selfTransform;

    private void Awake()
    {
        selfTransform = gameObject.transform;
        Actions.OnDeath.AddListener(Death);
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            if (active)
                StopCoroutine(fighting);
            active = value;
            fighting = fightingUpdate();
            StartCoroutine(fighting);
        }
        else
        {
            if (active)
                StopCoroutine(fighting);
            active = value;
        }
    }

    private IEnumerator fightingUpdate()
    {
        float addedTime = 0;
        while(active)
        {
            if(currentTarget == null)
            {
                currentTarget = UnitPool.instance.GetNearestFromPool(selfTransform.position, !Actions.Stats.Ally);
            }
            else if(Vector3.Distance(selfTransform.position, currentTarget.transform.position) > attackRange)
            {
                Actions.MoveToPosition(currentTarget.transform.position);
            } else
            {
                Actions.Movement.Stop(); 
                Actions.UseSkill(currentTarget.transform.position, 0);
                addedTime += 1 * Actions.Stats.GetStat(StatTag.CooldownRecovery);
                //addedTime += Actions.Stats.AttackSpeed - updateTime;
            }
            yield return new WaitForSeconds(updateTime + addedTime);
            addedTime = 0;
        }
    }

    private void Death(UnitActions action)
    {
        SetActive(false);
    }
}
