using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRecovery
{
    private UnitActions owner;
    //Utility
    private float vampirismRecoveryAmount = 0;
    //RecoveryStats
    private float vampirismRecoveryPerSecondMaximum = 0.1f;
    private float vampirismMaximumPerInstance = 0.1f;
    private float vampirismPerSecondMultiplier = 1;
    
    public UnitRecovery(UnitActions owner)
    {
        this.owner = owner;
        owner.LifeValue.OnReachingMaximum.AddListener(ClearLifeVampirismAmount);
        owner.LifeValue.OnReachingZero.AddListener(ClearLifeVampirismAmount);
    }

    public void Update(float deltaTime)
    {
        owner.LifeValue.AddValue(owner.Stats.GetStat(StatTag.lifeRegeneration) * deltaTime);
        float recoverAmount = Mathf.Clamp(vampirismRecoveryAmount, 0, owner.LifeValue.MaximumValue * vampirismRecoveryPerSecondMaximum) * deltaTime;
        owner.LifeValue.AddValue(recoverAmount);
        vampirismRecoveryAmount -= recoverAmount;
    }

    public void AddVampirismInstance(float value)
    {
        vampirismRecoveryAmount += value * owner.LifeValue.MaximumValue * vampirismMaximumPerInstance;
    }

    public void ClearLifeVampirismAmount()
    {
        vampirismRecoveryPerSecondMaximum = 0;
    }
}
