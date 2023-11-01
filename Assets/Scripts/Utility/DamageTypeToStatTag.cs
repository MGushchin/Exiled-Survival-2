using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageTypeToStatTag
{
    public static StatTag GetDamageTypeToStatTag(DamageType damage)
    {
        Dictionary<DamageType, StatTag> dict = new Dictionary<DamageType, StatTag>
        {
            { DamageType.Physical, StatTag.PhysicalDamage },
            { DamageType.Fire, StatTag.FireDamage },
            { DamageType.Cold, StatTag.ColdDamage },
            { DamageType.Lightning, StatTag.LightningDamage }
        };
        return dict[damage];
    }
}
