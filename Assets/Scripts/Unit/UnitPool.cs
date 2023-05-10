using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPool : MonoBehaviour
{
    public static UnitPool instance;
    [SerializeField]
    private List<UnitActions> allyPool = new List<UnitActions>();
    [SerializeField]
    private List<UnitActions> enemyPool = new List<UnitActions>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddToPool(UnitActions unit, bool ally)
    {
        if (ally)
            allyPool.Add(unit);
        else
            enemyPool.Add(unit);
    }

    public void RemoveFromPool(UnitActions unit, bool ally)
    {
        if (ally)
            allyPool.Remove(unit);
        else
            enemyPool.Remove(unit);
    }

    public UnitActions GetNearestFromPool(Vector3 position, bool ally)
    {
        List<UnitActions> targetPool;
        if (ally)
            targetPool = allyPool;
        else
            targetPool = enemyPool;
        if (targetPool.Count > 0)
        {
            UnitActions nearestUnit = targetPool[0];
            float nearestDistance = Vector3.Distance(position, nearestUnit.transform.position);
            foreach (UnitActions unit in targetPool)
            {
                float distance = Vector3.Distance(unit.transform.position, position);
                if (distance < nearestDistance)
                {
                    nearestUnit = unit;
                    nearestDistance = distance;
                }
            }
            return nearestUnit;
        }
        else
            return null;
    }
}
