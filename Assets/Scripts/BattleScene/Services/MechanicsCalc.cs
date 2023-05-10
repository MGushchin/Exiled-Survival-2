using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MechanicsCalc
{
    public static float CalculateDamageReduction(float hit, float armour)
    {
        float damageReduction = (armour) / (armour + 5 * hit);
        return damageReduction;
    }
}
