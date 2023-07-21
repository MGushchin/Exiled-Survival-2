using System.Collections;
using System.Collections.Generic;
using UIMarkers;
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
    private List<UnitActions> reserveBossUnitsPool;
    private List<UnitActions> activeBossUnitsPool;
    //Game rules
    private const float magicUnitsStatMult = 1.5f;
    private const float magicUnitsSizeMult = 1.25f;
    private const float rareUnitsStatMult = 2f;
    private const float rareUnitsSizeMult = 1.5f;
    private const float bossUnitsStatMult = 5;
    private const float bossUnitsSizeMult = 3;

    public void Init(int commonCount, int magicCount, int rareCount)
    {
        reserveCommonUnitsPool = new List<UnitActions>();
        activeCommonUnitsPool = new List<UnitActions>();
        reserveMagicUnitsPool = new List<UnitActions>();
        activeMagicUnitsPool = new List<UnitActions>();
        reserveRareUnitsPool = new List<UnitActions>();
        activeRareUnitsPool = new List<UnitActions>();
        reserveBossUnitsPool = new List<UnitActions>();
        activeBossUnitsPool = new List<UnitActions>();
        if (commonCount > 0)
        {
            createCommonUnit(commonCount);
        }
        if (magicCount > 0)
        {
            createMagicUnit(magicCount);
        }
        if (rareCount > 0)
        {
            createRareUnit(rareCount);
        }
    }

    private void createCommonUnit(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject unit = Instantiate(Prefab);
            UnitActions action;
            if (unit.GetComponent<UnitRoute>() == null)
                action = unit.GetComponent<UnitActions>();
            else
                action = unit.GetComponent<UnitRoute>().Actions;
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
            UnitActions action;
            if (unit.GetComponent<UnitRoute>() == null)
                action = unit.GetComponent<UnitActions>();
            else
                action = unit.GetComponent<UnitRoute>().Actions;
            //Visual
            SpriteRenderer renderer;
            renderer = action.Animations.gameObject.GetComponent<SpriteRenderer>(); //ѕереписать
            renderer.color = new Color(0, 0, 50);
            unit.transform.localScale = new Vector3(unit.transform.localScale.x * magicUnitsSizeMult, unit.transform.localScale.y * magicUnitsSizeMult, 1);
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
            UnitActions action;
            if (unit.GetComponent<UnitRoute>() == null)
                action = unit.GetComponent<UnitActions>();
            else
                action = unit.GetComponent<UnitRoute>().Actions;
            //Visual
            SpriteRenderer renderer;
            renderer = action.Animations.gameObject.GetComponent<SpriteRenderer>(); //ѕереписать
            renderer.color = new Color(153, 153, 0);
            unit.transform.localScale = new Vector3(unit.transform.localScale.x * rareUnitsSizeMult, unit.transform.localScale.y * rareUnitsSizeMult, 1);
            //Init
            action.InitUnit();
            //action.OnDeath.AddListener(ReturnToRarePool);
            reserveRareUnitsPool.Add(action);
            unit.SetActive(false);
        }
    }

    private void createBossUnit(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject unit = Instantiate(Prefab);
            UnitActions action;
            if (unit.GetComponent<UnitRoute>() == null)
                action = unit.GetComponent<UnitActions>();
            else
                action = unit.GetComponent<UnitRoute>().Actions;
            //Visual
            SpriteRenderer renderer;
            renderer = action.Animations.gameObject.GetComponent<SpriteRenderer>(); //ѕереписать
            renderer.color = new Color(50, 0, 0);
            unit.transform.localScale = new Vector3(unit.transform.localScale.x * bossUnitsSizeMult, unit.transform.localScale.y * bossUnitsSizeMult, 1);
            //Init
            action.InitUnit();
            //action.OnDeath.AddListener(ReturnToRarePool);
            reserveBossUnitsPool.Add(action);
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
        float life = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * magicUnitsStatMult;
        float damage = 1 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * magicUnitsStatMult;
        float armour = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * magicUnitsStatMult;
        startingStats.Add(new SetterStatData { Tag = StatTag.life, Base = life, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.PhysicalDamage, Base = damage, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.Armour, Base = armour, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.AttackRange, Base = magicUnitsSizeMult * 0.75f, Increase = 0, More = new List<float>() });
        unit.Stats.SetStats(startingStats);
        unit.Experience.SetLevel(GlobalData.instance.LevelData.MonsterLevel);
        //Name
        unit.gameObject.name += " (Magic)";
        //Drop adding
        GameObject expirienceDrop = DropableItemsFactory.Instance.CreateInactiveExperienceItem(GlobalData.instance.LevelData.MonsterLevel * magicUnitsStatMult);
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
        float life = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * rareUnitsStatMult;
        float damage = 1 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * rareUnitsStatMult;
        float armour = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * rareUnitsStatMult;
        startingStats.Add(new SetterStatData { Tag = StatTag.life, Base = life, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.PhysicalDamage, Base = damage, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.Armour, Base = armour, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.AttackRange, Base = rareUnitsSizeMult * 0.75f, Increase = 0, More = new List<float>() });
        unit.Stats.SetStats(startingStats);
        unit.Experience.SetLevel(GlobalData.instance.LevelData.MonsterLevel);
        //Name
        unit.gameObject.name += " (Rare)";
        //Drop adding
        GameObject expirienceDrop = DropableItemsFactory.Instance.CreateInactiveExperienceItem(GlobalData.instance.LevelData.MonsterLevel * rareUnitsStatMult);
        unit.Drop.AddItemToDrop(expirienceDrop);
        UnitPool.instance.AddToPool(unit, unit.Stats.Ally);
        return unit;
    }

    public UnitActions GetBossEnemy()
    {
        if (reserveBossUnitsPool.Count == 0)
            createBossUnit(1);
        UnitActions unit = reserveBossUnitsPool[0];
        reserveBossUnitsPool.RemoveAt(0);
        activeBossUnitsPool.Add(unit);
        unit.OnDeath.AddListener(ReturnToBossPool);
        List<SetterStatData> startingStats = new List<SetterStatData>();
        float life = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * bossUnitsStatMult;
        float damage = 1 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * bossUnitsStatMult;
        float armour = 10 * Mathf.Pow(1.25f, GlobalData.instance.LevelData.MonsterLevel - 1) * bossUnitsStatMult;
        startingStats.Add(new SetterStatData { Tag = StatTag.life, Base = life, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.PhysicalDamage, Base = damage, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.Armour, Base = armour, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.AttackRange, Base = bossUnitsSizeMult * 0.75f, Increase = 0, More = new List<float>() });
        startingStats.Add(new SetterStatData { Tag = StatTag.AreaOfEffect, Base = 1, Increase = 0, More = new List<float>() { bossUnitsSizeMult * 0.75f * 100 } });
        unit.Stats.SetStats(startingStats);
        unit.Experience.SetLevel(GlobalData.instance.LevelData.MonsterLevel);

        //Name
        unit.gameObject.name += " (Boss)";

        //Drop adding
        GameObject expirienceDrop = DropableItemsFactory.Instance.CreateInactiveExperienceItem(GlobalData.instance.LevelData.MonsterLevel * bossUnitsStatMult);
        unit.Drop.AddItemToDrop(expirienceDrop);
        UnitPool.instance.AddToPool(unit, unit.Stats.Ally);

        //Marker
        unit.Animations.gameObject.AddComponent(typeof(MarkerTarget));
        MarkerTarget marker = unit.Animations.gameObject.GetComponent<MarkerTarget>();
        marker.Init(MarkerType.Boss);

        return unit;
    }

    //ѕереработать повтор€ющийс€ код
    public void ReturnToCommonPool(UnitActions action)
    {
        reserveCommonUnitsPool.Add(action);
        activeCommonUnitsPool.Remove(action);
        action.OnDeath.RemoveListener(ReturnToCommonPool);
        action.OnTakingDamage.RemoveAllListeners();
    }

    public void ReturnToMagicPool(UnitActions action)
    {
        reserveMagicUnitsPool.Add(action);
        activeMagicUnitsPool.Remove(action);
        action.OnDeath.RemoveListener(ReturnToMagicPool);
        action.OnTakingDamage.RemoveAllListeners();
    }

    public void ReturnToRarePool(UnitActions action)
    {
        reserveRareUnitsPool.Add(action);
        activeRareUnitsPool.Remove(action);
        action.OnDeath.RemoveListener(ReturnToRarePool);
        action.OnTakingDamage.RemoveAllListeners();
    }

    public void ReturnToBossPool(UnitActions action)
    {
        reserveBossUnitsPool.Add(action);
        activeBossUnitsPool.Remove(action);
        action.OnDeath.RemoveListener(ReturnToBossPool);
        action.OnTakingDamage.RemoveAllListeners();
    }
}
