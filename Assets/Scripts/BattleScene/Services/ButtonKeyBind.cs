using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonKeyBind : MonoBehaviour
{
    public Button ButtonToPress;

    public void ClickButton()
    {
        //if(gameObject.activeSelf)
            ButtonToPress.onClick.Invoke();
    }
}
