using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EscMenuController : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
    }

    public void MenuToggle()
    {
        if(pauseMenuPanel.activeSelf)
        {
            if (settingsMenuPanel.activeSelf)
            {
                CloseSettings();
            }
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void OpenSettings()
    {
        settingsMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
