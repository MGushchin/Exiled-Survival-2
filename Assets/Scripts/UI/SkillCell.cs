using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCell : MonoBehaviour
{
    public Image Icon;
    public TMP_Text Name;
    public TMP_Text Description;


    public void SetCell(SkillMod data)
    {
        Icon.sprite = data.Icon;
        Name.text = data.Name;
        Description.text = data.Description;
    }
}
