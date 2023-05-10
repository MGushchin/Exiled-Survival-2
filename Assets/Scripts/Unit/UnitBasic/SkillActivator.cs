using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActivator : MonoBehaviour //Переписать как сервис перед UnitSkillsStorage
{
    public UnitSkillsStorage Storage; //Первоначально
    [SerializeField]
    private UnitActions owner;
    public List<GameObject> SkillObjects = new List<GameObject>();
    //[SerializeField]
    //private List<Skill> skillList = new List<Skill>();
    private List<int> autoCastIndexes = new List<int>();

    public void InitSkills()
    {
        foreach(GameObject skill in SkillObjects)
        {
            Skill currentSkill = skill.GetComponent<Skill>();
            //skillList.Add(currentSkill);
            currentSkill.InitSKill(owner);
        }
    }

    public void AddSkill(GameObject addedSkill)
    {
        Skill currentSkill = addedSkill.GetComponent<Skill>();
        addedSkill.transform.parent = transform;
        addedSkill.transform.localPosition = Vector3.zero;
        //skillList.Add(currentSkill);
        currentSkill.InitSKill(owner);
    }

    //public void UseSkill(Vector3 castPoint)
    //{
    //    foreach (int index in autoCastIndexes)
    //    {
    //        if(Storage.ActiveSkills[index] != null) // Временно
    //        Storage.ActiveSkills[index].UseSkill(castPoint);
    //    }
    //}

    public void UseSkill(Vector3 castPoint, int skillIndex)
    {
        if (Storage.ActiveSkills[skillIndex] != null) // Временно
            Storage.ActiveSkills[skillIndex].UseSkill(castPoint);
    }

    public void SetAutocast(int autocastSkill)
    {
        if (autoCastIndexes.Contains(autocastSkill))
            autoCastIndexes.Remove(autocastSkill);
        else
            autoCastIndexes.Add(autocastSkill);
    }

    public void StopUse(int index)
    {
        if (Storage.ActiveSkills[index] != null) // Временно
            Storage.ActiveSkills[index].StopUseSkill();
    }



}
