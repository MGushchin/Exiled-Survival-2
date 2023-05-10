using Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerSkillsInit : MonoBehaviour
{
    public UnitActions Owner;
    public SkillActivator Activator;
    public UnitSkillsStorage Storage;
    public PassiveMastery Passives;

    public void Start()
    {
        Passives.InitSKill(Owner);
        foreach (GameObject skillObject in Owner.SkillsActivation.SkillObjects) 
        {
            Skill skill = skillObject.GetComponent<Skill>();
            Owner.SkillsActivation.Storage.AddSkill(skill);
        }
    }
}
