using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractFactory: MonoBehaviour
{
    public GameObject Prefab;
    private List<UnitActions> reserveCommonUnitsPool;
    private List<UnitActions> activeCommonUnitsPool;
    private List<UnitActions> reserveMagicUnitsPool;
    private List<UnitActions> activeMagicUnitsPool;
    private List<UnitActions> reserveRareUnitsPool;
    private List<UnitActions> activeRareUnitsPool;
    //Game rules
    private const float magicUnitsMult = 1.5f;
    private const float rareUnitsMult = 2f;

    public void Init(int commonCount, int magicCount, int rareCount)
    {
        if(commonCount > 0)
        {
            reserveCommonUnitsPool = new List<UnitActions>();
            activeCommonUnitsPool = new List<UnitActions>();
            createCommonUnit(commonCount);
        }
        if (magicCount > 0)
        {
            reserveMagicUnitsPool = new List<UnitActions>();
            activeMagicUnitsPool = new List<UnitActions>();
            createMagicUnit(magicCount);
        }
        if (rareCount > 0)
        {
            reserveRareUnitsPool = new List<UnitActions>();
            activeRareUnitsPool = new List<UnitActions>();
            createRareUnit(rareCount);
        }
    }

    private void createCommonUnit(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject unit = Instantiate(Prefab);
            UnitActions action = unit.GetComponent<UnitActions>();
            action.InitUnit();
            //action.OnDeath.AddListener(ReturnToCommonPool);
            reserveCommonUnitsPool.Add(action);
            unit.SetActive(false);
        }
    }

    private void createMagicUnit(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject unit = Instantiate(Prefab);
            UnitActions action = unit.GetComponent<UnitActions>();
            //Visual
            SpriteRenderer renderer;
            renderer = action.Animations.GetComponent<SpriteRenderer>(); //Переписать
            renderer.color = new Color(0, 0, 50);
            unit.transform.localScale = new Vector3(unit.transform.localScale.x * 1.25f, unit.transform.localScale.y * 1.25f, 1);
            //Init
            action.InitUnit();
            //action.OnDeath.AddListener(ReturnToMagicPool);
            reserveMagicUnitsPool.Add(action);
            unit.SetActive(false);
        }
    }

    private void createRareUnit(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject unit = Instantiate(Prefab);
            UnitActions action = unit.GetComponent<UnitActions>();
            //Visual
            SpriteRenderer renderer;
            renderer = action.Animations.GetComponent<SpriteRenderer>(); //Переписать
            renderer.color = new Color(153, 153, 0);
            unit.transform.localScale = new Vector3(unit.transform.localScale.x * 1.5f, unit.transform.localScale.y * 1.5f, 1);
            //Init
            action.InitUnit();
            //action.OnDeath.AddListener(ReturnToRarePool);
            reserveRareUnitsPool.Add(action);
            unit.SetActive(false);
        }
    }

    public UnitActions GetCommonEnemy()
    {
        if (reserveCommonUnitsPool.Count == 0)
            createCommonUnit(10);
        UnitActions unit = reserveCommonUnitsPool[0];
        reserveCommonUnitsPool.RemoveAt(0);
        activeCommonUnitsPool.Add(unit);
        unit.OnDeath.AddListener(ReturnToCommonPool);
        List<SetterStatData> startingStats = new List<SetterStatData>();
        float life = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1);
        float damage = 1 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1);
        float armour = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1);
        startingStats.Add(new SetterStatData { Tag = StatTag.life, Base = life, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.PhysicalDamage, Base = damage, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.Armour, Base = armour, Increase = 0, More = new List<float>() });
        unit.Stats.SetStats(startingStats);
        unit.Experience.SetLevel(GlobalData.instance.LevelData.MonsterLevel);
        //Drop adding
        GameObject expirienceDrop = DropableItemsFactory.Instance.CreateInactiveExperienceItem(GlobalData.instance.LevelData.MonsterLevel);
        unit.Drop.AddItemToDrop(expirienceDrop);
        UnitPool.instance.AddToPool(unit, unit.Stats.Ally);
        return unit;
    }

    public UnitActions GetMagicEnemy()
    {
        if (reserveMagicUnitsPool.Count == 0)
            createMagicUnit(5);
        UnitActions unit = reserveMagicUnitsPool[0];
        reserveMagicUnitsPool.RemoveAt(0);
        activeMagicUnitsPool.Add(unit);
        unit.OnDeath.AddListener(ReturnToMagicPool);
        List<SetterStatData> startingStats = new List<SetterStatData>();
        float life = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * magicUnitsMult;
        float damage = 1 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * magicUnitsMult;
        float armour = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * magicUnitsMult;
        startingStats.Add(new SetterStatData { Tag = StatTag.life, Base = life, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.PhysicalDamage, Base = damage, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.Armour, Base = armour, Increase = 0, More = new List<float>() });
        unit.Stats.SetStats(startingStats);
        unit.Experience.SetLevel(GlobalData.instance.LevelData.MonsterLevel);
        //Drop adding
        GameObject expirienceDrop = DropableItemsFactory.Instance.CreateInactiveExperienceItem(GlobalData.instance.LevelData.MonsterLevel * magicUnitsMult);
        unit.Drop.AddItemToDrop(expirienceDrop);
        UnitPool.instance.AddToPool(unit, unit.Stats.Ally);
        return unit;
    }

    public UnitActions GetRareEnemy()
    {
        if (reserveRareUnitsPool.Count == 0)
            createRareUnit(1);
        UnitActions unit = reserveRareUnitsPool[0];
        reserveRareUnitsPool.RemoveAt(0);
        activeRareUnitsPool.Add(unit);
        unit.OnDeath.AddListener(ReturnToRarePool);
        List<SetterStatData> startingStats = new List<SetterStatData>();
        float life = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * rareUnitsMult;
        float damage = 1 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * rareUnitsMult;
        float armour = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * rareUnitsMult;
        startingStats.Add(new SetterStatData { Tag = StatTag.life, Base = life, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.PhysicalDamage, Base = damage, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.Armour, Base = armour, Increase = 0, More = new List<float>() });
        unit.Stats.SetStats(startingStats);
        unit.Experience.SetLevel(GlobalData.instance.LevelData.MonsterLevel);
        //Drop adding
        GameObject expirienceDrop = DropableItemsFactory.Instance.CreateInactiveExperienceItem(GlobalData.instance.LevelData.MonsterLevel * rareUnitsMult);
        unit.Drop.AddItemToDrop(expirienceDrop);
        UnitPool.instance.AddToPool(unit, unit.Stats.Ally);
        return unit;
    }


    public void ReturnToCommonPool(UnitActions action)
    {
        reserveCommonUnitsPool.Add(action);
        activeCommonUnitsPool.Remove(action);
        action.OnDeath.RemoveListener(ReturnToCommonPool);
    }

    public void ReturnToMagicPool(UnitActions action)
    {
        reserveMagicUnitsPool.Add(action);
        activeMagicUnitsPool.Remove(action);
        action.OnDeath.RemoveListener(ReturnToMagicPool);
    }

    public void ReturnToRarePool(UnitActions action)
    {
        reserveRareUnitsPool.Add(action);
        activeRareUnitsPool.Remove(action);
        action.OnDeath.RemoveListener(ReturnToRarePool);
    }
}
