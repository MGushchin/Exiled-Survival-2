using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameTimeController : MonoBehaviour
{
    public static GameTimeController instance;
    //Systems must be paused
    public PlayerInputControl InputControl;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        InputControl.SetInGameControl(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        InputControl.SetInGameControl(true);
    }
}
