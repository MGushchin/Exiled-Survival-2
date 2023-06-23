using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillActivator : MonoBehaviour //���������� ��� ������ ����� UnitSkillsStorage
{
    public UnitSkillsStorage Storage; //�������������
    [SerializeField]
    private UnitActions owner;
    public List<GameObject> SkillObjects = new List<GameObject>();
    //[SerializeField]
    //private List<Skill> skillList = new List<Skill>();
    public List<int> autoCastIndexes = new List<int>();
    private Vector3 castPosition = Vector3.zero;

    public void InitSkills()
    {
        foreach(GameObject skill in SkillObjects)
        {
            Skill currentSkill = skill.GetComponent<Skill>();
            //skillList.Add(currentSkill);
            currentSkill.InitSKill(owner);
        }
    }

    public void SetCastPoint(Vector3 castPosition)
    {
        this.castPosition = castPosition;
    }

    public void AddSkill(GameObject addedSkill)
    {
        Skill currentSkill = addedSkill.GetComponent<Skill>();
        addedSkill.transform.parent = transform;
        addedSkill.transform.localPosition = Vector3.zero;
        //skillList.Add(currentSkill);
        currentSkill.InitSKill(owner);
    }

    private void Update() //�������� ��������� ����������
    {
        autocast();
    }

    private void autocast()
    {
        foreach (int index in autoCastIndexes)
        {
            if (Storage.ActiveSkills[index] != null) // ��������
                Storage.ActiveSkills[index].UseSkill(castPosition);
        }
    }
    //public void UseSkill(Vector3 castPoint)
    //{
    //    foreach (int index in autoCastIndexes)
    //    {
    //        if(Storage.ActiveSkills[index] != null) // ��������
    //        Storage.ActiveSkills[index].UseSkill(castPoint);
    //    }
    //}

    public void UseSkill(Vector3 castPoint, int skillIndex)
    {
        if (Storage.ActiveSkills[skillIndex] != null) // ��������
            Storage.ActiveSkills[skillIndex].UseSkill(castPoint);
    }

    public void SetAutocast(int autocastSkill)
    {
        if (Storage.ActiveSkills[autocastSkill] != null)
        {
            if (autoCastIndexes.Contains(autocastSkill))
            {
                autoCastIndexes.Remove(autocastSkill);
            }
            else
            {
                autoCastIndexes.Add(autocastSkill);
            }
        }
    }

    public void StopUse(int index)
    {
        if (Storage.ActiveSkills[index] != null) // ��������
            Storage.ActiveSkills[index].StopUseSkill();
    }



}
