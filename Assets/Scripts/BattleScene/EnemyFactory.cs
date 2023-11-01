using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyFactory : MonoBehaviour
{
    [System.Serializable]
    private class enemyPrefab
    {
        public GameObject Prefab;
        public float SpawnChanceRate = 1;
    }

    private class enemyFactory
    {
        public AbstractFactory Factory;
        public float SpawnChanceRate = 1;

        public enemyFactory(AbstractFactory factory, float spawnChanceRate)
        {
            Factory = factory;
            SpawnChanceRate = spawnChanceRate;
        }
    }
    [SerializeField]
    private List<enemyPrefab> Prefabs = new List<enemyPrefab>();
    //private List<AbstractFactory> factories = new List<AbstractFactory>();
    private List<enemyFactory> factories = new List<enemyFactory>();

    public void CreatePool()
    {
        for(int i=0; i < Prefabs.Count; i++)
        {
            AbstractFactory factory = gameObject.AddComponent(typeof(AbstractFactory)) as AbstractFactory;
            factories.Add(new enemyFactory(factory, Prefabs[i].SpawnChanceRate));
            factory.Prefab = Prefabs[i].Prefab;
            factory.Init(10, 5, 2);
        }
    }

    public UnitActions CreateCommonEnemy()
    {
        AbstractFactory factory = getRandomFactory();
        UnitActions unit = factory.GetCommonEnemy();
        //UnitActions unit = factories[Random.Range(0, factories.Count)].GetCommonEnemy();
        return unit;
    }

    public UnitActions CreateMagicEnemy()
    {
        AbstractFactory factory = getRandomFactory();
        UnitActions unit = factory.GetMagicEnemy();
        //UnitActions unit = factories[Random.Range(0, factories.Count)].GetMagicEnemy();
        return unit;
    }

    public UnitActions CreateRareEnemy()
    {
        AbstractFactory factory = getRandomFactory();
        UnitActions unit = factory.GetRareEnemy();
        //UnitActions unit = factories[Random.Range(0, factories.Count)].GetRareEnemy();
        return unit;
    }

    public UnitActions CreateBossEnemy()
    {
        AbstractFactory factory = getRandomFactory();
        UnitActions unit = factory.GetBossEnemy();
        //UnitActions unit = factories[Random.Range(0, factories.Count)].GetBossEnemy();
        return unit;
    }

    private AbstractFactory getRandomFactory()
    {
        float fullWeight = 0;
        foreach(enemyFactory factory in factories)
        {
            fullWeight += factory.SpawnChanceRate;
        }
        float random = Random.Range(0, fullWeight);
        float currentWeight = 0;
        foreach (enemyFactory factory in factories)
        {
            currentWeight += factory.SpawnChanceRate;
            if(random <= currentWeight)
            {
                return factory.Factory;
            }
        }
        Debug.LogError("Random factories search error");
        return null;
    }
    //public void ReturnToPool(UnitActions action)
    //{
    //    reserveUnits.Add(action);
    //    activeUnits.Remove(action);
    //    action.OnDeath.RemoveListener(ReturnToPool);
    //}
}
