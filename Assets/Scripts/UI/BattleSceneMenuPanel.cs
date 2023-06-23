using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneMenuPanel : MonoBehaviour
{
    public DynamicSkillsDisplayPanel SkillsPanel;
    public void SetMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        SkillsPanel.SetEditMode(gameObject.activeSelf);
        if (gameObject.activeSelf)
            GameTimeController.instance.PauseGame();
        else
            GameTimeController.instance.ResumeGame();
    }
}
