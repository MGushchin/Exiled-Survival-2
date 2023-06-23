using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    private EnemyFactory factory;
    private Transform player;

    public void Init(EnemyFactory factory, Transform player)
    {
        this.factory = factory;
        this.player = player;
    }

    public void SpawnModAroundPlayer(Rarities rarity)
    {
        UnitActions enemy;
        switch(rarity)
        {
            case (Rarities.Common):
                {
                    enemy = factory.CreateCommonEnemy();
                }
                break;
            case (Rarities.Magic):
                {
                    enemy = factory.CreateMagicEnemy();
                }
                break;
            case (Rarities.Rare):
                {
                    enemy = factory.CreateRareEnemy();
                }
                break;
            default:
                {
                    enemy = factory.CreateCommonEnemy();
                    Debug.LogWarning("Common exception");
                }
                break;
        }
        initUnit(enemy);
    }

    public UnitActions SpawnBossAroundPlayer()
    {
        UnitActions boss;
        boss = factory.CreateBossEnemy();
        initUnit(boss);
        return boss;
    }

    private UnitActions initUnit(UnitActions unit)
    {
        unit.gameObject.SetActive(true);
        unit.transform.position = getSpawnPosition();
        UnitPool.instance.AddToPool(unit, unit.Stats.Ally);
        unit.Ressurect();
        unit.GetComponent<CommonEnemyBehaviour>().SetActive(true); //переписать
        unit.OnTakingDamage.AddListener(DamageIndicatorSpawner.instance.IndicateDamage);

        return unit;
    }

    private Vector3 getSpawnPosition()
    {
        float x, y;
        if (Random.Range(0, 2) == 0)
            x = 10;
        else
            x = -10;
        if (Random.Range(0, 2) == 0)
            y = 10;
        else
            y = -10;
        Vector3 randomPosition = new Vector3(player.position.x + x, player.position.y + y, 0);
        randomPosition = GlobalData.instance.LevelData.Map.GetCloser((int)randomPosition.x, (int)randomPosition.y);
        return randomPosition;
    }
}
