using GameStateManagement;
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
        GameStateManager.Instance.SetState(new PlayingState("UI_testing"));
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
        if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
