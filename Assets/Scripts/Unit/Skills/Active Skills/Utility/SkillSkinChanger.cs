using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSkinChanger : MonoBehaviour
{
    public SpriteRenderer SkillSkin;
    public List<Sprite> skin = new List<Sprite>();

    public void SetSkin(int index)
    {
        SkillSkin.sprite = skin[index];
    }

    public void SetColor()
    {
        //Добавить при необходимости
    }
}
