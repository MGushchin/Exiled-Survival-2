using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyFactory : MonoBehaviour
{
    public List<GameObject> Prefabs = new List<GameObject>();
    //public int PoolSize = 10;
    //private List<UnitActions> reserveUnits = new List<UnitActions>();
    //private List<UnitActions> activeUnits = new List<UnitActions>();
    private List<AbstractFactory> factories = new List<AbstractFactory>();

    public void CreatePool()
    {
        //for (int i = 0; i < PoolSize; i++)
        //{
        //    GameObject unit = Instantiate(Prefab);
        //    UnitActions action = unit.GetComponent<UnitActions>();
        //    action.InitUnit();
        //    action.OnDeath.AddListener(ReturnToPool);
        //    reserveUnits.Add(action);
        //    unit.SetActive(false);
        //}
        for(int i=0; i < Prefabs.Count; i++)
        {
            AbstractFactory factory = gameObject.AddComponent(typeof(AbstractFactory)) as AbstractFactory;
            factories.Add(factory);
            factory.Prefab = Prefabs[i];
            factory.Init(10, 5, 2);
        }
    }

    public UnitActions CreateCommonEnemy()
    {
        UnitActions unit = factories[Random.Range(0, factories.Count)].GetCommonEnemy();
        return unit;
    }

    public UnitActions CreateMagicEnemy()
    {
        UnitActions unit = factories[Random.Range(0, factories.Count)].GetMagicEnemy();
        return unit;
    }

    public UnitActions CreateRareEnemy()
    {
        UnitActions unit = factories[Random.Range(0, factories.Count)].GetRareEnemy();
        return unit;
    }


    //public void ReturnToPool(UnitActions action)
    //{
    //    reserveUnits.Add(action);
    //    activeUnits.Remove(action);
    //    action.OnDeath.RemoveListener(ReturnToPool);
    //}
}
