using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class LevelUpButton : MonoBehaviour
{
    public UnityEvent OnClick = new UnityEvent();
    public TMP_Text HotketText;
    public TMP_Text SkillPointsText;
    private PlayerInputControl control;

    private void Start()
    {
        control = FindObjectOfType<PlayerInputControl>();
        control.OnOpenLevelUp.AddListener(Click);
        UpdateHotkey();
    }

    public void UpdateSkillPointsText()
    {
        int value = GlobalData.instance.PlayerData.SkillPoints;
        SkillPointsText.text = value.ToString();
    }

    public void UpdateHotkey()
    {
        HotketText.text = control.LevelUpButton.ToString();
    }

    public void Click()
    {
        gameObject.SetActive(false);
            OnClick.Invoke();
    }
}
