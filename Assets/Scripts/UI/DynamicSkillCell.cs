using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DynamicSkillCell : MonoBehaviour
{
    public Image SkillImage;
    public Slider CooldownMask;
    public TMP_Text HotkeyText;
    public EditingDynamicSkills Edit;
    public GameObject AutoImage;
    public bool Auto = false;
    private Sprite clearSprite;

    private void Start()
    {
        //CooldownMask.maxValue = 100;
        //CooldownMask.minValue = 0;
        //CooldownMask.value = 0;
        clearSprite = SkillImage.sprite;
    }

    public void SetHotkeyText(string text)
    {
        HotkeyText.text = text;
    }

    public void SetSkillImage(Sprite skillIcon)
    {
        SkillImage.sprite = skillIcon;
    }

    public void SetCooldownData(float cooldownPercent)
    {
        CooldownMask.value = cooldownPercent;
    }

    public void SetAuto(bool value)
    {
        Auto = value;
        AutoImage.SetActive(Auto);
    }

    public void Clear()
    {
        SkillImage.sprite = clearSprite;
        CooldownMask.value = 0;
    }
}
