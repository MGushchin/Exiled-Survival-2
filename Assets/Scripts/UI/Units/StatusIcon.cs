using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIcon : MonoBehaviour
{
    public Image Image;
    public TMP_Text Text;

    public void SetIcon(Sprite image, string text)
    {
        Image.sprite = image;
        Text.text = text;
    }
}
