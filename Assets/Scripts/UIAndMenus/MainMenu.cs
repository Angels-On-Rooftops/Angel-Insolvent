using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject settingsPanel;

    private void Awake()
    {
        settingsPanel.SetActive(false);
    }
    public void OnPlayButton()
    {
        Debug.Log("Hit Play Button");
    }

    public void OnSettingsButton()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnSettingsDoneButton()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }
    public void OnQuitButton()
    {
        Application.Quit();
        Debug.Log("Hit Quit Button");
    }

    //Demo scene load functions
    public void LoadPauseAndDialogueDemo()
    {
        SceneManager.LoadScene("UI_testing");
    }
}