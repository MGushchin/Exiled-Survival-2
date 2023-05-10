using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenChange : MonoBehaviour
{
    private GameObject currentScreen;
    public GameObject MainScreen;
    public GameObject AreaScreen;
    public GameObject HeroScreen;

    private void Awake()
    {
        currentScreen = MainScreen;
    }

    public void ChangeScreen(GameObject newScreen)
    {
        currentScreen.SetActive(false);
        currentScreen = newScreen;
        currentScreen.SetActive(true);
    }

    public void SelectAreaScreen()
    {
        ChangeScreen(AreaScreen);
    }

    public void SelectHeroScreen()
    {
        ChangeScreen(HeroScreen);
    }
}
