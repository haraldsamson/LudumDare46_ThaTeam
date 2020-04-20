using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
    }

    void Pause()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
