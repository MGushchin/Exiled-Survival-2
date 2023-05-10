using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitExperience
{
    private float currentXP = 0;
    public float CurrentExperience => currentXP;
    private float maximumXP = 10;
    public float MaxiumExperience => maximumXP;
    private int level = 1;
    public int Level => level;

    public void SetLevel(int level)
    {
        this.level = level;
        maximumXP = 10 * Mathf.Pow(1.25f, level - 1);
    }

    public bool AddExperience(float experience)
    {
        currentXP += experience;
        if (currentXP > maximumXP)
        {
            levelUp();
            return true;
        }
        else
            return false;
    }

    private void levelUp()
    {
        level++;
        currentXP -= maximumXP;
        maximumXP *= 1.25f;
    }
}
