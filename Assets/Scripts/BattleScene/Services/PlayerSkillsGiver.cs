using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerSkillsGiver : MonoBehaviour
{
    public UnityEvent OnChange = new UnityEvent();
    [System.Serializable]
    private class skillData
    {
        [SerializeField]
        private string skillName;
        public string SkillName => skillName;
        [SerializeField]
        private bool enabled = true;
        public bool Enabled => enabled;
        public SkillMod BaseMod;
        public List<SkillMod> SkillMods;
        public GameObject SkillPrefab;
    }
    [SerializeField]
    private List<skillData> skillsData = new List<skillData>();

    private UnitActions player;
    [SerializeField]
    private List<SkillMod> avaibleMods = new List<SkillMod>();
    [SerializeField]
    private List<SkillMod> unavaibleMods = new List<SkillMod>();
    private Dictionary<string, SkillMod> allModsByName = new Dictionary<string, SkillMod>();
    private List<SkillMod> preparedMods = new List<SkillMod>();
    private List<string> bannedSkillMods = new List<string>();
    private Dictionary<SkillMod, skillData> skillDataByBaseSkillMod = new Dictionary<SkillMod, skillData>();

    public void Init(UnitActions player)
    {
        this.player = player;
        for (int i = 0; i < skillsData.Count; i++)
        {
            if (skillsData[i].Enabled)
            {
                skillDataByBaseSkillMod.Add(skillsData[i].BaseMod, skillsData[i]);
                SkillMod mod = Instantiate(skillsData[i].BaseMod);
                mod.UpdateDescription();
                skillsData[i].BaseMod = mod;
                allModsByName.Add(mod.Name, mod);
                if (checkModRequirements(mod))
                {
                    avaibleMods.Add(mod);
                }
                else
                {
                    unavaibleMods.Add(mod);
                }
                for (int j = 0; j < skillsData[i].SkillMods.Count; j++)
                {
                    mod = Instantiate(skillsData[i].SkillMods[j]);
                    skillsData[i].SkillMods[j] = mod;
                    allModsByName.Add(mod.Name, mod);
                    mod.UpdateDescription();
                    if (checkModRequirements(mod))
                    {
                        avaibleMods.Add(mod);
                    }
                    else
                    {
                        unavaibleMods.Add(mod);
                    }
                }
            }
        }
        //Passive mastery section
        Skill skill = player.SkillsActivation.Storage.Passives;
        LearnAlreadyCreatedBaseSkill(skill.Name, skill, true);
        foreach (GameObject skillObject in player.SkillsActivation.SkillObjects) //Оптимизировать при необходимости
        {
            skill = skillObject.GetComponent<Skill>();
            LearnAlreadyCreatedBaseSkill(skill.Name, skill, false);
        }
        OnChange.Invoke();
    }

    public List<SkillMod> GetRandomMods(int count)
    {
        preparedMods.Clear();
        while (count > 0 && avaibleMods.Count > 0)
        {
            count--;
            SkillMod mod = avaibleMods[Random.Range(0, avaibleMods.Count)];
            avaibleMods.Remove(mod);
            preparedMods.Add(mod);
        }
        foreach (SkillMod mod in preparedMods) //?? переписать
            avaibleMods.Add(mod);
        return preparedMods;
    }

    //public List<SkillMod> GetRandomActiveSkillMods(int count)
    //{
    //    preparedMods.Clear();
    //    while (count > 0 && avaibleMods.Count > 0)
    //    {
    //        count--;
    //        SkillMod mod = avaibleMods[Random.Range(0, avaibleMods.Count)];
    //        avaibleMods.Remove(mod);
    //        preparedMods.Add(mod);
    //    }
    //    foreach (SkillMod mod in preparedMods) //?? переписать
    //        avaibleMods.Add(mod);
    //    return preparedMods;
    //}

    //public List<SkillMod> GetRandomPassiveSkillMods(int count)
    //{
    //    preparedMods.Clear();
    //    while (count > 0 && avaibleMods.Count > 0)
    //    {
    //        count--;
    //        SkillMod mod = avaibleMods[Random.Range(0, avaibleMods.Count)];
    //        avaibleMods.Remove(mod);
    //        preparedMods.Add(mod);
    //    }
    //    foreach (SkillMod mod in preparedMods) //?? переписать
    //        avaibleMods.Add(mod);
    //    return preparedMods;
    //}

    public void LearnSkillMod(string skillModName)
    {
        if (allModsByName.ContainsKey(skillModName))
        {
            SkillMod mod = allModsByName[skillModName];
            //if (mod.Level < mod.MaximumLevel)
            //    mod.UpgradeLevel();
            if (mod.Level == 0)
            {
                //player.Skills.Storage.AddSkillMod(mod);
                if (mod.BaseMod)
                {
                    addSkill(mod);
                    avaibleMods.Remove(mod);
                    //mod.ParentSkill.BaseSkillMod.UpgradeLevel();
                }
                player.SkillsActivation.Storage.AddSkillMod(mod);
                foreach (string excludedMod in mod.SkillModsForExclusion)
                {
                    bannedSkillMods.Add(excludedMod);
                }
            }
            mod.ParentSkill.ApplyUpgrade(mod.Name, mod.Level);
            //player.Skills.Storage.AddSkillMod(mod);
            if (mod.Level == mod.MaximumLevel)
                avaibleMods.Remove(mod);
            mod.ParentSkill.BaseSkillMod.UpgradeLevel();
            checkAllSkillRequirements();
        }
        OnChange.Invoke();
    }

    public void LearnAlreadyCreatedBaseSkill(string skillModName, Skill alreadyCreated, bool hidden) // Возможно переписать
    {
        if (allModsByName.ContainsKey(skillModName))
        {
            SkillMod mod = allModsByName[skillModName];
            avaibleMods.Remove(mod);
            if (mod.Level == 0)
            {
                mod.SetParentSkill(alreadyCreated);
                for (int i = 0; i < skillsData.Count; i++)
                {
                    if (mod.Name == skillsData[i].SkillName)
                    {
                        GameObject skillObject = alreadyCreated.gameObject;
                        alreadyCreated.SetBaseSkillMod(skillsData[i].BaseMod);
                        foreach (SkillMod skillMod in skillsData[i].SkillMods)
                        {
                            skillMod.SetParentSkill(alreadyCreated);
                        }
                        foreach (string excludedMod in mod.SkillModsForExclusion)
                        {
                            bannedSkillMods.Add(excludedMod);
                        }
                        if(!hidden)
                            player.SkillsActivation.Storage.AddSkill(alreadyCreated);
                        break;
                    }
                }
                mod.ParentSkill.BaseSkillMod.UpgradeLevel();
            }
            mod.ParentSkill.ApplyUpgrade(mod.Name, mod.Level);
            player.SkillsActivation.Storage.AddSkillMod(mod);
            checkAllSkillRequirements();
        }
        else
            Debug.LogWarning("Key exceprion " + skillModName);
        OnChange.Invoke();
    }

    public void RemovePassiveSkills()
    {

    }

    private void excludeSkillMods(List<string> excludedMods)
    {
        foreach (string excludedMod in excludedMods)
        {
            bannedSkillMods.Add(excludedMod);
            avaibleMods.Remove(allModsByName[excludedMod]);
            unavaibleMods.Add(allModsByName[excludedMod]);
        }
    }

    private void addSkill(SkillMod mod)
    {
        bool find = false;
        for (int i = 0; i < skillsData.Count; i++)
        {
            if (mod.Name == skillsData[i].SkillName)
            {
                find = true;
                GameObject skillObject = Instantiate(skillsData[i].SkillPrefab);
                Skill skill = skillObject.GetComponent<Skill>();
                skill.SetBaseSkillMod(skillsData[i].BaseMod);
                mod.SetParentSkill(skill);
                foreach (SkillMod skillMod in skillsData[i].SkillMods)
                {
                    skillMod.SetParentSkill(skill);
                }
                player.SkillsActivation.AddSkill(skillObject);
                player.SkillsActivation.Storage.AddSkill(skill); //Переписать
                if (skill.ActiveSkill)
                {
                    if (player.SkillsActivation.Storage.ActiveSkillsCount >= player.SkillsActivation.Storage.SkillSlots)
                        removeSkillsFromPool(true);
                    //else
                    //    Debug.Log($"Active skill: Skills count {player.SkillsActivation.Storage.ActiveSkillsCount} Storage skill slots {player.SkillsActivation.Storage.SkillSlots}");
                }
                else
                {
                    if (player.SkillsActivation.Storage.PassiveSkillsCount >= player.SkillsActivation.Storage.SkillSlots)
                        removeSkillsFromPool(false);
                    //else
                    //    Debug.Log($"Passive skill: Skills count {player.SkillsActivation.Storage.PassiveSkillsCount} Storage skill slots {player.SkillsActivation.Storage.SkillSlots}");
                }
                break;
            }
        }
        if (!find)
            Debug.LogError("Not Finded skill mods to set parent");
    }

    private void checkAllSkillRequirements()
    {
        for (int i = 0; i < unavaibleMods.Count; i++)
        {
            if (checkModRequirements(unavaibleMods[i]) && !bannedSkillMods.Contains(unavaibleMods[i].Name))
            {
                avaibleMods.Add(unavaibleMods[i]);
                unavaibleMods.RemoveAt(i);
                i--;
            }
        }
    }

    private bool checkModRequirements(SkillMod mod)
    {
        bool met = true;
        foreach (SkillModRequirements req in mod.SkillsModsRequirements)
        {
            if (getSkillModLevel(req.Name) < req.Level)
            {
                met = false;
                break;
            }
        }
        return met;
    }

    private int getSkillModLevel(string skillModName)
    {
        if (allModsByName.ContainsKey(skillModName))
        {
            SkillMod mod = allModsByName[skillModName];
            return mod.Level;
        }
        else
        {
            return 0;
        }
    }

    private void removeSkillsFromPool(bool active)
    {
        for (int i = 0; i < avaibleMods.Count; i++)
        {
            if (avaibleMods[i].BaseMod)
            {
                skillData data;
                for (int j = 0; j < skillsData.Count; j++) //Переписать
                {
                    if (avaibleMods[i].Name == skillsData[j].SkillName)
                    {
                        data = skillsData[j];
                        if (data.SkillPrefab.GetComponent<Skill>().ActiveSkill == active) //Переписать
                        {
                            //unavaibleMods.Add(avaibleMods[i]);
                            avaibleMods.RemoveAt(i);
                            i--;
                        }
                        break;
                    }
                }
            }
        }
    }
}
