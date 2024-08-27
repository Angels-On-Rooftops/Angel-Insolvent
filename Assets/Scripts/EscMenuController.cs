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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseMenuPanel.activeSelf)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Quit()
    {
        if(Application.isEditor)
        {
            EditorApplication.ExitPlaymode();
        }
        Application.Quit();
    }
}
