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
    private float spawnRate = 2;
    private float spawnRateLimit = 10;
    private float magicMobChance = 0;
    private float rareMobChance = 0;
    private float bossSpawnTime = 60;
    #region Boss counting
    private int defeatedBosses = 0;
    private int bossCount = 3;
    private int spawnedBosses = 0;
    #endregion
    #region Coroutines
    IEnumerator spawnCoroutine;
    IEnumerator difficultyCoroutine;
    IEnumerator bossCoroutine;
    #endregion

    public void InitScenario(MapData map, EnemySpawn spawner, Transform player)
    {
        this.map = map;
        this.spawner = spawner;
        this.player = player;
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
        yield return new WaitForSeconds(bossSpawnTime);
        spawnBoss();
        spawnedBosses++;
        afterBossSpawn();
    }

    private void afterBossSpawn()
    {
        if(spawnedBosses + defeatedBosses < bossCount)
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
        spawner.SpawnModAroundPlayer(rarity);
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
        boss.OnDeath.RemoveListener(OnBossDeath);
        if (defeatedBosses == bossCount)
            Debug.Log("Winning");
    }
}
