using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{
    public PlayerSkillsGiver ActiveSkillGiver;
    public PlayerSkillsGiver PassiveSkillGiver;
    public List<SkillCell> Cells = new List<SkillCell>();
    public GameObject LevelUpPanel;

    private bool learningActiveSkill = false;
    private List<SkillMod> preparedSkillMods = new List<SkillMod>();
    private UnitActions player;

    public void Init(UnitActions player)
    {
        this.player = player;
    }

    public void EnableLevelUpPanel()
    {
        if (GlobalData.instance.PlayerData.TakeSkillPoint())
        {
            if (/*player.SkillsActivation.Storage.ActiveSkills.Length < player.SkillsActivation.Storage.SkillSlots && */GlobalData.instance.PlayerData.Level % 2 == 0)
            {
                preparedSkillMods = ActiveSkillGiver.GetWeightedRandomMods(4);
                learningActiveSkill = true;
            }
            else
            {
                preparedSkillMods = PassiveSkillGiver.GetWeightedRandomMods(4);
                learningActiveSkill = false;
            }
            LevelUpPanel.SetActive(true);
            for (int i = 0; i < Cells.Count; i++)
            {
                if (i < preparedSkillMods.Count)
                {
                    Cells[i].SetCell(preparedSkillMods[i]);
                }
                else
                    Cells[i].gameObject.SetActive(false);
            }
            GameTimeController.instance.PauseGame();
        }
    }

    public void LearnSkillMod(int index)
    {
        SkillMod mod = preparedSkillMods[index];
        if(learningActiveSkill)
            ActiveSkillGiver.LearnSkillMod(mod.Name);
        else
            PassiveSkillGiver.LearnSkillMod(mod.Name);
        preparedSkillMods.Clear();
        EnableLevelUpPanel(); //Рекурсия, возможно логически переписать
        LevelUpPanel.SetActive(false);
        GameTimeController.instance.ResumeGame();
    }
}
