using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsUpgrade : MonoBehaviour
{
    public Skill UpgradeableSkill;
    private UnitActions owner;
    private UnitSkillsStorage storage;
    private int skillLevel = 1;
    public int SkillLevel => skillLevel;
    [SerializeField]
    private List<SkillMod> upgrades = new List<SkillMod>();
    [SerializeField]
    private List<SkillMod> lockedUpgrades = new List<SkillMod>();

    public void InitUpgrade(UnitSkillsStorage storage)
    {
        this.storage = storage;
        skillLevel = 1;
        foreach(SkillMod mod in upgrades)
        {
            SkillMod tempMod = Instantiate(mod);
            //tempMod.SetParentUpgrade(this);
            lockedUpgrades.Add(tempMod);
        }
        CheckAllRequirements();
    }

    public void ApplyUpgrade(SkillMod mod)
    {
        //if (mod.Level == 0)
        //    storage.AddLearnedMod(mod);
        mod.UpgradeLevel();
        UpgradeableSkill.ApplyUpgrade(mod);
        //foreach(StatMod statMod in mod.UnitStatMods)
        //{
        //    owner.Stats.AddStat(statMod.Tag, statMod.TagType, statMod.Value);
        //}
        skillLevel++;
        if(mod.Level == mod.MaximumLevel)
        {
            //storage.RemoveFromUnlocked(mod);
        }
        CheckAllRequirements();
    }

    public void CheckAllRequirements()
    {
        //int k = 0;
        //for(int i = 0; i < lockedUpgrades.Count; i++)
        //{
        //    bool metRequirements = true;
        //    foreach (SkillModRequirements requirement in lockedUpgrades[i].SkillsRequirements)
        //    {
        //        if (requirement.Level > storage.GetSkillLevel(requirement.Name))
        //        {
        //            metRequirements = false;
        //            break;
        //        }
        //    }
        //    if(metRequirements)
        //    {
        //        storage.AddUnlockedSkillMod(lockedUpgrades[i]);
        //        lockedUpgrades.RemoveAt(i);
        //        k++;
        //        i = -1;
        //        if (k > 10000000) //Debug
        //        {
        //            Debug.LogWarning("k > 10000000");
        //            break;
        //        }
        //    }
        //}
    }
}
