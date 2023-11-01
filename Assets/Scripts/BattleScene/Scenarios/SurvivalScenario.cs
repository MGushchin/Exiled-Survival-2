using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalScenario : MonoBehaviour
{
    private bool active = false;
    private EnemySpawn spawner;
    private Transform player;
    private MapData map;
    private IEnumerator updateCoroutine;
    private float timer = 0;
    private float difficultyTimer = 60;
    private float difficultyRaiseTime = 60;
    private float spawnRate = 1;
    private float spawnRateLimit = 10;
    private float magicMobChance = 0;
    private float rareMobChance = 0;
    private float bossSpawnTime = 300;
    #region Boss counting
    private int defeatedBosses = 0;
    private int bossCount = 3;
    private int spawnedBosses = 0;
    #endregion
    #region Coroutines
    IEnumerator spawnCoroutine;
    IEnumerator difficultyCoroutine;
    IEnumerator bossCoroutine;
    IEnumerator magicPackCoroutine;
    IEnumerator rareCoroutine;
    #endregion
    #region UnitsCounting
    private List<UnitActions> aliveUnits = new List<UnitActions>();
    private int killCount = 0;
    #endregion
    #region Events
    float rareMobSpawnTime = 60;
    float magicPackSpawnTime = 30;
    int magicPackMobCount = 4;
    #endregion

    public void InitScenario(MapData map, EnemySpawn spawner, Transform player)
    {
        this.map = map;
        this.spawner = spawner;
        this.player = player;
        StatisticsDisplay.instance.RegisterData("KillCount", "Kills: " + killCount);
        StatisticsDisplay.instance.RegisterData("BossSpawnTime", "Boss spawn: " + bossSpawnTime);
        StatisticsDisplay.instance.RegisterData("BossKillCount", "Boss killed: " + defeatedBosses + "/" + bossCount);
    }

    public void StartScenario(int stage)
    {
        active = true;

        #region Debug
        //bossCount = 1;
        //bossSpawnTime = 10;
        #endregion

        spawnCoroutine = spawnUpdate();
        difficultyCoroutine = difficultyUpdate();
        bossCoroutine = bossTimer();
        StartCoroutine(spawnCoroutine);
        StartCoroutine(difficultyCoroutine);
        StartCoroutine(bossCoroutine);
        magicPackCoroutine = magicPackTimer();
        rareCoroutine = rareTimer();
        StartCoroutine(magicPackCoroutine);
        StartCoroutine(rareCoroutine);

    }

    private IEnumerator levelUpdate()
    {
        while (active)
        {
            spawnEnemy();
            yield return new WaitForSeconds(1 / spawnRate);
            timer += 1 / spawnRate;
            if (timer >= difficultyTimer)
            {
                timer -= difficultyTimer;
                //spawnRate -= 0.5f;
                //spawnRate += 0.1f;
                magicMobChance = Mathf.Clamp(magicMobChance + 1, 0, 50);
                rareMobChance = Mathf.Clamp(rareMobChance + 0.25f, 0, 20);
            }
            //spawnRate += 0.1f;
        }
    }

    private IEnumerator spawnUpdate()
    {
        while (active)
        {
            yield return new WaitForSeconds(1 / spawnRate);
            spawnEnemy();
        }
    }

    private IEnumerator difficultyUpdate()
    {
        while (active)
        {
            yield return new WaitForSeconds(difficultyRaiseTime);
            increasingDifficulty();
        }
    }

    private IEnumerator bossTimer()
    {
        float timer = bossSpawnTime;
        while (bossSpawnTime > 0)
        {
            yield return new WaitForSeconds(1);
            timer -= 1;
            StatisticsDisplay.instance.UpdateStat("BossSpawnTime", "Boss spawn: " + timer);
        }
        //yield return new WaitForSeconds(bossSpawnTime);

        spawnBoss();
        spawnedBosses++;
        afterBossSpawn();
    }

    private IEnumerator magicPackTimer()
    {
        float timer = magicPackSpawnTime;

        while (active)
        {
            yield return new WaitForSeconds(1);
            timer -= 1;
            if (timer <= 0)
            {
                spawnEnemyPack(Rarities.Magic, magicPackMobCount);
                timer = timer = magicPackSpawnTime;
            }
        }
    }

    private IEnumerator rareTimer()
    {
        float timer = rareMobSpawnTime;

        while (active)
        {
            yield return new WaitForSeconds(1);
            timer -= 1;
            if (timer <= 0)
            {
                spawnEnemy(Rarities.Rare);
                timer = timer = magicPackSpawnTime;
            }
        }
    }

    private void afterBossSpawn()
    {
        if (spawnedBosses + defeatedBosses < bossCount)
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = bossTimer();
            StartCoroutine(bossCoroutine);
        }
    }

    private void spawnEnemy()
    {
        Rarities rarity;
        int chance = Random.Range(1, 101);
        if (chance < rareMobChance)
        {
            rarity = Rarities.Rare;
        }
        else if (chance < magicMobChance)
        {
            rarity = Rarities.Magic;
        }
        else
            rarity = Rarities.Common;
        UnitActions spawnedUnit = spawner.SpawnModAroundPlayer(rarity);
        spawnedUnit.OnDeath.AddListener(OnUnitDeath);
        aliveUnits.Add(spawnedUnit);
    }

    private void spawnEnemy(Rarities rarity)
    {
        UnitActions spawnedUnit = spawner.SpawnModAroundPlayer(rarity);
        spawnedUnit.OnDeath.AddListener(OnUnitDeath);
        aliveUnits.Add(spawnedUnit);
    }

    private void spawnEnemyPack(Rarities rarity, int count)
    {
        List<UnitActions> spawnedUnits;

        spawnedUnits = spawner.SpawnMobPackAroundPlayer(rarity, count);
        foreach (UnitActions spawnedUnit in spawnedUnits)
        {
            spawnedUnit.OnDeath.AddListener(OnUnitDeath);
            aliveUnits.Add(spawnedUnit);
        }
    }

    private void spawnBoss()
    {
        spawner.SpawnBossAroundPlayer().OnDeath.AddListener(OnBossDeath);
    }

    private void increasingDifficulty()
    {
        timer -= difficultyTimer;
        spawnRate = Mathf.Clamp(spawnRate + 0.1f, 0, spawnRateLimit);
        magicMobChance = Mathf.Clamp(magicMobChance + 1, 0, 50);
        rareMobChance = Mathf.Clamp(rareMobChance + 0.25f, 0, 20);
    }

    public void OnBossDeath(UnitActions boss)
    {
        defeatedBosses++;
        spawnedBosses--;
        StatisticsDisplay.instance.UpdateStat("BossKillCount", "Boss killed: " + defeatedBosses + "/" + bossCount);
        boss.OnDeath.RemoveListener(OnBossDeath);
        if (defeatedBosses == bossCount)
            Debug.Log("Winning");
    }

    public void OnUnitDeath(UnitActions unit)
    {
        killCount++;
        aliveUnits.Remove(unit);
        StatisticsDisplay.instance.UpdateStat("KillCount", "Kills: " + killCount);
    }
}
