using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfTheGameExecute : MonoBehaviour
{
    public GameObject LevelCompletedPanel;
    public GameObject GameOverPanel;

    public void RenderGameOverPanel(UnitActions unit)
    {
        GameOverPanel.SetActive(true);
    }

    public void RenderCompleteLevelPanel() //Нет реализации
    {
        LevelCompletedPanel.SetActive(true);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(0);
    }

    public void CompleteGame()
    {
        SceneManager.LoadScene(0);
    }
}
