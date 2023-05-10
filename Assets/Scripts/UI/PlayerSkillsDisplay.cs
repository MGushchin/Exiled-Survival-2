using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSkillsDisplay : MonoBehaviour //Под рефакторинг
{
    public List<Image> SkillsImages = new List<Image>();
    public List<GameObject> SkillsObjects = new List<GameObject>();
    public List<TMP_Text> SkillsTexts = new List<TMP_Text>();
    private SkillActivator skills;
    private IEnumerator updateCoroutine;


    public void Init(SkillActivator skills)
    {
        this.skills = skills;
        updateCoroutine = slowUpdate();
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < 5; i++)
        {
            if (skills.Storage.ActiveSkills[i] != null)
            {
                SkillsImages[i].color = new Color(255, 255, 255, 200);
                SkillsImages[i].sprite = skills.Storage.ActiveSkills[i].SkillIcon;
                SkillsTexts[i].text = skills.Storage.ActiveSkills[i].Level.ToString();
            }
            else
            {
                SkillsImages[i].color = new Color(255, 255, 255, 0);
                //SkillsImages[i].sprite = null;
                SkillsTexts[i].text = "";
            }
            if (skills.Storage.PassiveSkills[i] != null)
            {
                SkillsImages[i + 5].color = new Color(255, 255, 255, 200);
                SkillsImages[i + 5].sprite = skills.Storage.PassiveSkills[i].SkillIcon;
                SkillsTexts[i + 5].text = skills.Storage.PassiveSkills[i].Level.ToString();
            }
            else
            {
                SkillsImages[i + 5].color = new Color(255, 255, 255, 0);
                //SkillsImages[i + 5].sprite = null;
                SkillsTexts[i + 5].text = "";
            }
        }
    }

    private IEnumerator slowUpdate()
    {
        while (gameObject.activeSelf)
        {
            for (int i = 0; i < 5; i++)
            {
                if (skills.Storage.ActiveSkills[i] != null)
                {
                    SkillsObjects[i].SetActive(true);
                    SkillsImages[i].sprite = skills.Storage.ActiveSkills[i].SkillIcon;
                    SkillsTexts[i].text = skills.Storage.ActiveSkills[i].Level.ToString();
                }
                else
                {
                    SkillsObjects[i].SetActive(false);
                }
                if (skills.Storage.PassiveSkills[i] != null)
                {
                    SkillsObjects[i + 5].SetActive(true);
                    SkillsImages[i + 5].sprite = skills.Storage.PassiveSkills[i].SkillIcon;
                    SkillsTexts[i + 5].text = skills.Storage.PassiveSkills[i].Level.ToString();
                }
                else
                {
                    SkillsObjects[i + 5].SetActive(false);
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }
}
