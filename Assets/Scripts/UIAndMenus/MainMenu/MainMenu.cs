using System.Collections;
using System.Collections.Generic;
using GameStateManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject mainMenuPanel;

    public void OnPlayButton()
    {
        GameStateManager.Instance.SetState(new PlayingState());
    }

    public void OnSettingsButton()
    {
        mainMenuPanel.SetActive(false);
        GameManager.escMenu.OpenSettings();
    }

    public void OnQuitButton()
    {
        if (!Application.isEditor)
            System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
