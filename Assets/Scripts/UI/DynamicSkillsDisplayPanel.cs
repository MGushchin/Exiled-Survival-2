using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSkillsDisplayPanel : MonoBehaviour
{
    public EditingDynamicSkills Edit;
    public List<DynamicSkillCell> Cells = new List<DynamicSkillCell>();
    private UnitActions player;

    private void Start()
    {
        PlayerInputControl control = FindObjectOfType<PlayerInputControl>(); // Временно
        for (int i=0; i < control.SkillsButtons.Length; i++)
        {
            Cells[i].SetHotkeyText(control.SkillsButtons[i].ToString());
        }
    }

    public void SetPlayer(UnitActions player)
    {
        this.player = player;
    }

    public void SetEditMode(bool value)
    {
        Edit.SetEditMode(value);
    }

    public void ChangeCells(int cell1, int cell2)
    {
        player.SkillsActivation.Storage.ChangeActiveSkillsSlots(cell1, cell2);
    }

    private void Update()
    {
        skillStatusUpdate();
    }

    private void skillStatusUpdate()
    {
        for(int i=0; i < player.SkillsActivation.Storage.ActiveSkills.Length; i++)
        {
            if (player.SkillsActivation.Storage.ActiveSkills[i] != null) // Переписать
            {
                float cooldownTime = (player.SkillsActivation.Storage.ActiveSkills[i].Cooldown * 100 / player.SkillsActivation.Storage.ActiveSkills[i].SkillCooldown);
                Cells[i].SetCooldownData(cooldownTime);
                Cells[i].SetSkillImage(player.SkillsActivation.Storage.ActiveSkills[i].SkillIcon);

                if(player.SkillsActivation.autoCastIndexes.Contains(i) != Cells[i].Auto) //Переписать
                    Cells[i].SetAuto(player.SkillsActivation.autoCastIndexes.Contains(i));

            }
            else
            {
                Cells[i].Clear();
            }
        }
    }
}
