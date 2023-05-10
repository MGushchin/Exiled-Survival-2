using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Skill
{
    private struct projectile
    {
        public Hit hit;
        public ProjectilePierce pierce;
        public ProjectileMoving move;
        public DisappearingDelay projectileDisappearing;
        public Hit explosopnHit;
        public DisappearingDelay explosionDisppearing;
    }
    public GameObject ProjectilePrefab;
    public GameObject ExplosionPrefab;
    private Transform selfTransform;
    //Utility
    private Queue<projectile> fireballsPool = new Queue<projectile>();
    private IEnumerator cooldownCoroutine;
    //Skill Params
    private CombinedStat damageModifier = new CombinedStat(0, 0, new List<float>());
    private CombinedStat projectileDamageModifier = new CombinedStat(0, 0, new List<float> { 0.6f });
    private CombinedStat explosionDamageModifier = new CombinedStat(0, 0, new List<float> { 0.3f });
    private CombinedStat attackSpeedModifier = new CombinedStat(0, 0, new List<float>());
    private CombinedStat projectileCountModifier = new CombinedStat(1, 0, new List<float>());
    private CombinedStat projectileSpeedModifier = new CombinedStat(1, 0, new List<float>());
    private CombinedStat areaOfEffectModifier = new CombinedStat(1, 0, new List<float>());
    private float baseSkillCooldown = 1;
    private float baseCriticalStrikeChance = 5;
    //Utility Params
    private float projectileLifeTime => 1 / projectileSpeedModifier.ModValue;
    private int pierceCount = 1;
    private float projectilesDegree = 30;
    private float resultSkillCooldown => baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    private float minimumFireDegree = 30;
    private float maximumFireDegree = 180;
    #region UtilityLinks
    private int resultProjectilesCount => (int)owner.Stats.GetAdvancedStat(StatTag.ProjectileCount).ModValueWithAddedParams(projectileCountModifier);
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private int resultPierceCount => (int)(pierceCount + owner.Stats.GetAdvancedStat(StatTag.Pierce).Value);
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;
        updateProjectilesCount(resultProjectilesCount);
        owner.Stats.OnStatChanged[StatTag.ProjectileCount].AddListener(updateProjectilesCount); // Переписать
        owner.Stats.OnStatChanged[StatTag.AreaOfEffect].AddListener(updateAreaOfEffectMod); // Переписать
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            List<projectile> projectiles = new List<projectile>();
            int projectilesCount = resultProjectilesCount;
            for (int i = 0; i < projectilesCount; i++)
                projectiles.Add(fireballsPool.Dequeue());
            //Setup Hits
            projectiles = setupProjectiles(castPoint, projectiles);
            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(baseSkillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);
            //Debug.Log("Cooldown = " + baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.cooldownRecovery).ValueWithAddedParams(attackSpeedModifier))));
            for (int i = 0; i < projectiles.Count; i++)
                fireballsPool.Enqueue(projectiles[i]);
            return true;
        }
        else
            return false;
    }

    private List<HitData> getHitData()
    {
        List<HitData> hits = new List<HitData>();
        HitData projectileHit = owner.Stats.GetHitData(baseCriticalStrikeChance, new List<StatTag> { StatTag.ProjectileDamage });
        HitData explosionHit = owner.Stats.GetHitData(baseCriticalStrikeChance, new List<StatTag> { StatTag.AreaDamage });
        //Projectile
        projectileHit.PhysicalDamage *= damageModifier.ModValue * projectileDamageModifier.ModValue; //Переписать бы
        //projectileHit.FireDamage *= damageModifier.ModValue * projectileDamageModifier.ModValue; //Переписать бы
        //projectileHit.ColdDamage *= damageModifier.ModValue * projectileDamageModifier.ModValue; //Переписать бы
        //projectileHit.LightningDamage *= damageModifier.ModValue * projectileDamageModifier.ModValue; //Переписать бы
        hits.Add(projectileHit);
        //Explosion
        explosionHit.PhysicalDamage *= damageModifier.ModValue * explosionDamageModifier.ModValue; //Переписать бы
        //explosionHit.FireDamage *= damageModifier.ModValue * explosionDamageModifier.ModValue; //Переписать бы
        //explosionHit.ColdDamage *= damageModifier.ModValue * explosionDamageModifier.ModValue; //Переписать бы
        //explosionHit.LightningDamage *= damageModifier.ModValue * explosionDamageModifier.ModValue; //Переписать бы
        hits.Add(explosionHit);
        return hits;
    }

    private List<projectile> setupProjectiles(Vector3 castPoint, List<projectile> projectiles)
    {
        float currentDegree = 0;
        List<HitData> hits = getHitData();
        for (int i = 0; i < projectiles.Count; i++)
        {
            //Setup transform data
            projectiles[i].hit.SelfTransform.position = selfTransform.position;
            projectiles[i].hit.SelfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - selfTransform.position.y, castPoint.x - selfTransform.position.x) * Mathf.Rad2Deg - 90) + currentDegree); //Переписать
            //Set Hits
            //Set Porjectiles
            projectiles[i].hit.SetHit(hits[0]);
            projectiles[i].pierce.SetPierce(resultPierceCount);
            projectiles[i].hit.SetActiveHit(true);
            //Set Explosion
            projectiles[i].explosopnHit.SetHit(hits[1]);
            if (i % 2 == 0)
                currentDegree = (currentDegree + projectilesDegree) * -1;
            else
                currentDegree = currentDegree * -1;
        }
        return projectiles;
    }

    private void updateProjectilesCount(float unitProjectiles)
    {
        projectilesDegree = Mathf.Clamp(30 * resultProjectilesCount, minimumFireDegree, maximumFireDegree) / resultProjectilesCount;
        updateProjectilesPool();
    }

    private void updateAreaOfEffectMod(float areaOfEffect)
    {
        foreach(projectile proj in fireballsPool)
        {
            float scale = owner.Stats.GetAdvancedStat(StatTag.AreaOfEffect).ModValueWithAddedParams(areaOfEffectModifier);
            proj.hit.SelfTransform.localScale = new Vector3(scale, scale, scale);
            proj.explosopnHit.SelfTransform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void updateProjectilesPool()
    {
        int projectilePoolCount = (int)(((1 / resultSkillCooldown) * projectileLifeTime) * resultProjectilesCount + resultProjectilesCount) + 1;
        if (projectilePoolCount > fireballsPool.Count)
        {
            for (int i = fireballsPool.Count; i < projectilePoolCount; i++)
            {
                GameObject temporalProjectile = Instantiate(ProjectilePrefab);
                GameObject temporalExplosion = Instantiate(ExplosionPrefab);
                projectile temporalFireball = new projectile();
                //Projectile
                temporalFireball.hit = temporalProjectile.GetComponent<Hit>();
                temporalFireball.pierce = temporalProjectile.GetComponent<ProjectilePierce>();
                temporalFireball.move = temporalProjectile.GetComponent<ProjectileMoving>();
                temporalFireball.projectileDisappearing = temporalProjectile.GetComponent<DisappearingDelay>();
                temporalFireball.hit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
                temporalProjectile.SetActive(false);
                //Explosion
                temporalFireball.explosopnHit = temporalExplosion.GetComponent<Hit>();
                temporalFireball.explosionDisppearing = temporalExplosion.GetComponent<DisappearingDelay>();
                temporalFireball.explosopnHit.OnFeedbackReceived.AddListener(owner.TakeHitFeedback);
                temporalExplosion.SetActive(false);
                //Setup
                temporalFireball.pierce.OnLastPierce.AddListener(temporalFireball.explosopnHit.SetActiveHitAtPosition);
                temporalFireball.pierce.OnLastPierce.AddListener(temporalFireball.projectileDisappearing.DisappearNow);
                fireballsPool.Enqueue(temporalFireball);
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
        foreach (projectile proj in fireballsPool)
        {
            proj.hit.OnFeedbackReceived.RemoveListener(owner.TakeHitFeedback);
            Destroy(proj.hit.gameObject);
        }
        fireballsPool.Clear();
        StopCoroutine(cooldownCoroutine);
        owner.Stats.OnStatChanged[StatTag.ProjectileCount].RemoveListener(updateProjectilesCount);
        owner.Stats.OnStatChanged[StatTag.AreaOfEffect].RemoveListener(updateAreaOfEffectMod);
    }

    private IEnumerator cooldownRecovery(float time)
    {
        while (cooldown > 0)
        {
            yield return new WaitForEndOfFrame();
            cooldown -= Time.deltaTime;
        }
    }

    public override void ApplyUpgrade(string name, int level)
    {
        switch (name)
        {
            case ("Fireball Damage"):
                {
                    damageModifier.AddIncreaseValue(10);
                }
                break;
            case ("Fireball Attack Speed"):
                {
                    attackSpeedModifier.AddIncreaseValue(5);
                }
                break;
            case ("Fireball Area"):
                {
                    areaOfEffectModifier.AddIncreaseValue(10);
                }
                break;
            case ("Fireball Pierce"):
                {
                    pierceCount++;
                }
                break;
            default:
                {
                    Debug.LogWarning("Apply upgrade exception");
                }
                break;
        }
    }
}
