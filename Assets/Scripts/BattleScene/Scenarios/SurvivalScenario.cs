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
    private float levelUpTime = 60;
    private float spawnRate = 2;
    private float magicMobChance = 10;
    private float rareMobChance = 0;

    public void InitScenario(MapData map, EnemySpawn spawner, Transform player)
    {
        this.map = map;
        this.spawner = spawner;
        this.player = player;
    }

    public void StartScenario(int stage)
    {
        active = true;
        updateCoroutine = levelUpdate();
        StartCoroutine(updateCoroutine);
    }

    private IEnumerator levelUpdate()
    {
        while (active)
        {
            spawnEnemy();
            yield return new WaitForSeconds(1 / spawnRate);
            timer += 1 / spawnRate;
            if (timer >= levelUpTime)
            {
                GlobalData.instance.LevelData.AddMonsterLevel(1);
                timer -= levelUpTime;
                //spawnRate -= 0.5f;
                spawnRate += 0.1f;
                magicMobChance = Mathf.Clamp(magicMobChance + 1, 0, 50);
                rareMobChance = Mathf.Clamp(rareMobChance + 0.25f, 0, 20);
            }
            //spawnRate += 0.1f;
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
}
