using Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkillsStorage : MonoBehaviour
{
    public Skill[] ActiveSkills => activeSkills;
    public Skill[] PassiveSkills => passiveSkills;
    public PassiveMastery Passives => passives;
    private const int skillSlots = 5;
    public int SkillSlots => skillSlots;
    [SerializeField]
    private Skill[] activeSkills = new Skill[skillSlots];
    [SerializeField]
    private Skill[] passiveSkills = new Skill[skillSlots];
    [SerializeField]
    private PassiveMastery passives;
    private int activeSkillsCount = 0; //Переписать
    public int ActiveSkillsCount => activeSkillsCount;
    private int passiveSkillsCount = 0; //Переписать
    public int PassiveSkillsCount => passiveSkillsCount;

    private Dictionary<Skill, List<SkillMod>> storage = new Dictionary<Skill, List<SkillMod>>();

    public void AddSkill(Skill skill)
    {
        if (!storage.ContainsKey(skill))
        {
            if (skill.ActiveSkill)
            {
                if (activeSkillsCount < 5)
                {
                    storage.Add(skill, new List<SkillMod>());
                    activeSkills[activeSkillsCount] = skill;
                    activeSkillsCount++;
                }
                else
                    Debug.LogWarning("Array overflow");
            }
            else
            {
                if (passiveSkillsCount < 5)
                {
                    storage.Add(skill, new List<SkillMod>());
                    passiveSkills[passiveSkillsCount] = skill;
                    passiveSkillsCount++;
                }
                else
                    Debug.LogWarning("Array overflow");
            }
        }
        else
            Debug.Log("Повторное влкючение базового SkillMod");
    }

    public void AddSkillMod(SkillMod mod)
    {
        if (storage.ContainsKey(mod.ParentSkill))
        {
            storage[mod.ParentSkill].Add(mod);
        }
        else if(mod.ParentSkill != passives) //Переписать
            Debug.LogWarning(string.Format("Skill storage не содерижить ключ родительского навыка {0}", mod.name));
    }

    public void ChangeActiveSkillsSlots(int skill1, int skill2)
    {
        Skill temp = activeSkills[skill1];
        activeSkills[skill1] = activeSkills[skill2];
        activeSkills[skill2] = temp;
    }
}
