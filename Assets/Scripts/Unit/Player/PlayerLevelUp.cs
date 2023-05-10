using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUp : MonoBehaviour
{
    public PlayerSkillsGiver SkillGiver;
    public List<SkillCell> Cells = new List<SkillCell>();
    public GameObject LevelUpPanel;

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
            if(player.SkillsActivation.Storage.ActiveSkills.Length < player.SkillsActivation.Storage.SkillSlots && GlobalData.instance.PlayerData.Level % 2 == 0)
                preparedSkillMods = SkillGiver.GetRandomMods(4);
            else
                preparedSkillMods = SkillGiver.GetRandomMods(4);
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
            Time.timeScale = 0; //Перенести в глобальную систему
        }
    }

    public void LearnSkillMod(int index)
    {
        SkillMod mod = preparedSkillMods[index];
        SkillGiver.LearnSkillMod(mod.Name);
        preparedSkillMods.Clear();
        EnableLevelUpPanel(); //Рекурсия, возможно логически переписать
        LevelUpPanel.SetActive(false);
        Time.timeScale = 1; //Перенести в глобальную систему
    }
}
