using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class EscMenuController : MonoBehaviour
{
    [SerializeField]
    private InputAction pauseAction;

    [SerializeField]
    private GameObject pauseMenuPanel;

    [SerializeField]
    private GameObject settingsMenuPanel;

    [SerializeField]
    private GameObject savePromptPanel;

    public GameObject getPauseMenuPanel()
    {
        return pauseMenuPanel;
    }

    public GameObject getSettingsMenuPanel()
    {
        return settingsMenuPanel;
    }

    public GameObject getSavePromptPanel()
    {
        return savePromptPanel;
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
        savePromptPanel.SetActive(false);
    }

    private void OnEnable()
    {
        pauseAction.performed += PauseToggle;
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.performed -= PauseToggle;
        pauseAction.Disable();
    }

    public void PauseToggle(CallbackContext c)
    {
        if (GameStateManager.Instance.CurrentState is PlayingState)
        {
            GameStateManager.Instance.SetState(new GameStateManagement.PauseState(this));
        }
        else if (GameStateManager.Instance.CurrentState is not MainMenuState)
        {
            settingsMenuPanel.GetComponentInChildren<SettingsController>().SaveSettings();
            GameStateManager.Instance.SetState(new PlayingState());
        } else if(GameStateManager.Instance.CurrentState is MainMenuState)
        {
            settingsMenuPanel.GetComponentInChildren<SettingsController>().SaveSettings();
            GameStateManager.Instance.SetState(new MainMenuState());
        }
    }

    public void OnResumeButtonClick()
    {
        GameStateManager.Instance.SetState(new PlayingState());
    }

    public void OpenSettings()
    {
        settingsMenuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);

        settingsMenuPanel.GetComponentInChildren<Button>()?.Select();
        settingsMenuPanel.GetComponentInChildren<Button>()?.onClick.Invoke();
    }

    public void CloseSettings()
    {
        if(GameStateManager.Instance.CurrentState is not MainMenuState)
        {
            settingsMenuPanel.SetActive(false);
            pauseMenuPanel.SetActive(true);
            pauseMenuPanel.GetComponentsInChildren<Button>().First().Select();
        } else
        {
            settingsMenuPanel.SetActive(false);
            PauseToggle(new CallbackContext());
        }
    }

    public void CloseSavePrompt()
    {
        savePromptPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        pauseMenuPanel.GetComponentsInChildren<Button>().First().Select();
    }

    public void QuitToMainMenu()
    {
        savePromptPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        savePromptPanel.GetComponentsInChildren<Button>().Last().Select();
    }
}
