using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : Skill
{
    private struct projectile
    {
        public Hit hit;
        public ProjectilePierce pierce;
        public ProjectileMoving move;
        public DisappearingDelay dissapearing;
    }
    public GameObject ProjectilePrefab;
    private Transform selfTransform;
    //Utility
    private Queue<projectile> projectilesPool = new Queue<projectile>();
    private IEnumerator cooldownCoroutine;
    //Skill Params
    //private CombinedStat damageModifier = new CombinedStat(0, 0, new List<float>());
    //private CombinedStat attackSpeedModifier = new CombinedStat(0, 0, new List<float>());
    private CombinedStat projectileCountModifier = new CombinedStat(3, 0, new List<float>());
    //private CombinedStat projectileSpeedModifier = new CombinedStat(1.5f, 0, new List<float>());
    private float baseSkillCooldown = 1;
    //private float baseCriticalStrikeChance = 5;
    //Utility Params
    private float projectileLifeTime => 3;
    private int pierceCount = 0;
    private float projectilesDegree = 30;
    private float ResultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    private float minimumFireDegree = 10;
    private float maximumFireDegree = 360;
    #region UtilityLinks
    private int resultProjectilesCount => (int)owner.Stats.GetAdvancedStat(StatTag.ProjectileCount).ModValueWithAddedParams(projectileCountModifier);
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private int resultPierceCount => (int)(pierceCount + owner.Stats.GetAdvancedStat(StatTag.Pierce).Value);
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;
        Debug.Log(gameObject.name);
        Debug.Log(skillOwner);
        Debug.Log(owner);
        updateProjectilesCount(resultProjectilesCount);
        owner.Stats.OnStatChanged[StatTag.ProjectileCount].AddListener(updateProjectilesCount);
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            List<projectile> projectiles = new List<projectile>();
            int projectilesCount = resultProjectilesCount;
            for (int i = 0; i < projectilesCount; i++)
                projectiles.Add(projectilesPool.Dequeue());
            //Setup Hits
            projectiles = setupProjectiles(castPoint, projectiles);
            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(baseSkillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);
            //Debug.Log("Cooldown = " + baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.cooldownRecovery).ValueWithAddedParams(attackSpeedModifier))));
            for (int i = 0; i < projectiles.Count; i++)
                projectilesPool.Enqueue(projectiles[i]);
            return true;
        }
        else
            return false;
    }

    protected override HitData getHitData()
    {
        Debug.LogError("Not refactored code");
        HitData hit = owner.Stats.GetHitData();
        //hit.PhysicalDamage *= damageModifier.ModValue; //Переписать бы
        //hit.FireDamage *= damageModifier.ModValue; //Переписать бы
        //hit.ColdDamage *= damageModifier.ModValue; //Переписать бы
        //hit.LightningDamage *= damageModifier.ModValue; //Переписать бы
        return hit;
    }

    private List<projectile> setupProjectiles(Vector3 castPoint, List<projectile> projectiles)
    {
        float currentDegree = 0;
        float degreeOffset = Random.Range(0, 359);
        HitData hit = getHitData();
        for (int i = 0; i < projectiles.Count; i++)
        {
            //Setup transform data
            projectiles[i].hit.SelfTransform.position = selfTransform.position;
            projectiles[i].hit.SelfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - selfTransform.position.y, castPoint.x - selfTransform.position.x) * Mathf.Rad2Deg - 90) + currentDegree + degreeOffset); //Переписать
            //Set Hits
            projectiles[i].hit.SetHit(hit);
            projectiles[i].pierce.SetPierce(resultPierceCount);
            projectiles[i].hit.gameObject.SetActive(true); //переписать
            if (i % 2 == 0)
                currentDegree = (currentDegree + projectilesDegree) * -1;
            else
                currentDegree = currentDegree * -1;
        }
        return projectiles;
    }

    private void updateProjectilesCount(float unitProjectiles)
    {
        projectilesDegree = Mathf.Clamp(120 * resultProjectilesCount, minimumFireDegree, maximumFireDegree) / resultProjectilesCount;
        updateProjectilesPool();
    }

    private void updateProjectilesPool()
    {
        Debug.Log("Skill cooldown " + baseSkillCooldown + " Cooldown recovery speed " + resultCooldownRecoverySpeed+ " Result: " + baseSkillCooldown * resultCooldownRecoverySpeed);
        Debug.Log("Lifetime " + projectileLifeTime + " Result " + (int)(baseSkillCooldown * resultCooldownRecoverySpeed * projectileLifeTime));
        int projectilePoolCount = (int)(((1 / ResultSkillCooldown) * projectileLifeTime) * resultProjectilesCount + resultProjectilesCount) + 1;
        Debug.Log("Pool count 1 " + (int)(baseSkillCooldown * resultCooldownRecoverySpeed * projectileLifeTime) * resultProjectilesCount);
        Debug.Log("Pool count 2 " + projectilePoolCount);
        if (projectilePoolCount > projectilesPool.Count)
        {
            for (int i = projectilesPool.Count; i < projectilePoolCount; i++)
            {
                GameObject temporal = Instantiate(ProjectilePrefab);
                projectile temporalProjectile = new projectile();
                temporalProjectile.hit = temporal.GetComponent<Hit>();
                temporalProjectile.pierce = temporal.GetComponent<ProjectilePierce>();
                temporalProjectile.move = temporal.GetComponent<ProjectileMoving>();
                temporalProjectile.dissapearing = temporal.GetComponent<DisappearingDelay>();
                temporalProjectile.hit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
                projectilesPool.Enqueue(temporalProjectile);
                temporal.SetActive(false);
            }
        }
    }

    public override void StopUseSkill()
    {
        base.StopUseSkill();
    }

    public override void RemoveSkill()
    {
        base.RemoveSkill();
        foreach (projectile proj in projectilesPool)
        {
            proj.hit.OnFeedbackReceived.RemoveListener(owner.TakeHitFeedback);
            Destroy(proj.hit.gameObject);
        }
        projectilesPool.Clear();
        StopCoroutine(cooldownCoroutine);
        owner.Stats.OnStatChanged[StatTag.ProjectileCount].RemoveListener(updateProjectilesCount);
    }

    private IEnumerator cooldownRecovery(float time)
    {
        while (cooldown > 0)
        {
            yield return new WaitForEndOfFrame();
            cooldown -= Time.deltaTime;
        }
    }

    public override void ApplyUpgrade(SkillMod mod)
    {
        //base.ApplyUpgrade(name, level);
        switch (mod.name)
        {
            case ("Ethereal Weapon Damage"):
                {
                    damageModifier.AddIncreaseValue(10);
                }
                break;
            case ("Ethereal Weapon Attack Speed"):
                {
                    attackSpeedModifier.AddIncreaseValue(5);
                }
                break;
            case ("Ethereal Weapon Pierce"):
                {
                    pierceCount++;
                }
                break;
            default:
                {

                }
                break;
        }
    }
}
