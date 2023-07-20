using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShot : Skill
{
    private struct skillHit
    {
        public HitMasterSlave hit;
        public ProjectilePierce pierce;
        public ProjectileMoving move;
        public DisappearingDelay dissapearing;
    }
    public GameObject ProjectilePrefab;
    public GameObject DangerZonePrefab;
    private Transform selfTransform;
    //Utility
    private Queue<skillHit> projectilesPool = new Queue<skillHit>();
    private IEnumerator cooldownCoroutine;
    private VectorDangerZoneRenderer dangerZoneRenderer;
    //Skill Params
    private CombinedStat projectileCountModifier = new CombinedStat(1, 0, new List<float>());
    private CombinedStat projectileSpeedModifier = new CombinedStat(0, 0, new List<float>());
    //Utility Params
    private float projectileLifeTime => 1 / projectileSpeedModifier.ModValue;
    private int pierceCount = 0;
    private float projectilesDegree = 30;
    private float minimumFireDegree = 30;
    private float maximumFireDegree = 180;
    #region UtilityLinks
    private int resultProjectilesCount => (int)owner.Stats.GetAdvancedStat(StatTag.ProjectileCount).ModValueWithAddedParams(projectileCountModifier);
    private float resultSkillCooldown => skillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).ModValueWithAddedParams(attackSpeedModifier)));
    private float resultCooldownRecoverySpeed => (1 / owner.Stats.GetAdvancedStat(StatTag.CooldownRecovery).Value);
    private int resultPierceCount => (int)(pierceCount + owner.Stats.GetAdvancedStat(StatTag.Pierce).Value);
    #endregion

    public override void InitSKill(UnitActions skillOwner)
    {
        owner = skillOwner;
        selfTransform = gameObject.transform;
        skillCooldown = resultSkillCooldown;
        cooldown = 0;
        updateProjectilesCount(owner.Stats.GetAdvancedStat(StatTag.ProjectileCount).Value);
        dangerZoneRenderer = Instantiate(DangerZonePrefab, transform).GetComponent<VectorDangerZoneRenderer>();
        owner.Stats.OnStatChanged[StatTag.ProjectileCount].AddListener(updateProjectilesCount);
        hitTags = new List<StatTag> { StatTag.ProjectileDamage, StatTag.PhysicalDamage, StatTag.AttackDamage };
        skillCooldown = 4;
    }

    public override bool UseSkill(Vector3 castPoint)
    {
        if (cooldown <= 0)
        {
            dangerZoneRenderer.SetActive(new Vector2(0.1f, 5), 1);
            List<skillHit> projectiles = new List<skillHit>();
            int projectilesCount = resultProjectilesCount;
            for (int i = 0; i < projectilesCount; i++)
                projectiles.Add(projectilesPool.Dequeue());
            //Setup Hits
            projectiles = setupProjectiles(castPoint, projectiles);
            //Self skill setup
            skillCooldown = resultSkillCooldown;
            cooldown = skillCooldown;
            cooldownCoroutine = cooldownRecovery(skillCooldown * resultCooldownRecoverySpeed);
            StartCoroutine(cooldownCoroutine);
            //Debug.Log("Cooldown = " + baseSkillCooldown * (1 / (owner.Stats.GetAdvancedStat(StatTag.cooldownRecovery).ValueWithAddedParams(attackSpeedModifier))));
            for (int i = 0; i < projectiles.Count; i++)
                projectilesPool.Enqueue(projectiles[i]);
            return true;
        }
        else
            return false;
    }

    //private IEnumerator casting()
    //{
    //    yield return new WaitForSeconds();
    //}

    private List<skillHit> setupProjectiles(Vector3 castPoint, List<skillHit> projectiles)
    {
        float currentDegree = 0;
        HitData hit = getHitData();
        HitMasterSlave master = projectiles[projectiles.Count - 1].hit;
        for (int i = 0; i < projectiles.Count; i++)
        {
            //Setup transform data
            projectiles[i].hit.SelfTransform.position = selfTransform.position;
            projectiles[i].hit.SelfTransform.rotation = Quaternion.Euler(selfTransform.rotation.eulerAngles.x, selfTransform.rotation.eulerAngles.y, (Mathf.Atan2(castPoint.y - selfTransform.position.y, castPoint.x - selfTransform.position.x) * Mathf.Rad2Deg - 90) + currentDegree); //Переписать
            //Set Hits
            projectiles[i].hit.SetMaster(master);
            projectiles[i].hit.SetHit(hit);
            projectiles[i].pierce.SetPierce(resultPierceCount);
            projectiles[i].hit.SetActiveHit(true);
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

    private void updateProjectilesPool()
    {
        int projectilePoolCount = (int)(((1 / resultSkillCooldown) * projectileLifeTime) * resultProjectilesCount + resultProjectilesCount) + 1;
        if (projectilePoolCount > projectilesPool.Count)
        {
            for (int i = projectilesPool.Count; i < projectilePoolCount; i++)
            {
                GameObject temporal = Instantiate(ProjectilePrefab);
                skillHit temporalProjectile = new skillHit();
                temporalProjectile.hit = temporal.GetComponent<HitMasterSlave>();
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
        foreach (skillHit proj in projectilesPool)
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
    }
}
