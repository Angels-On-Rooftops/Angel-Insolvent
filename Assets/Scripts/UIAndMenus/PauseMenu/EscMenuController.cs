using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class EscMenuController : MonoBehaviour
{
    [SerializeField] private InputAction pauseAction;

    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsMenuPanel;
    [SerializeField] private GameObject savePromptPanel;

    [SerializeField] public AudioSource audioSource { get; private set; }

    public GameObject getPauseMenuPanel() { return pauseMenuPanel; }
    public GameObject getSettingsMenuPanel() { return settingsMenuPanel; }
    public GameObject getSavePromptPanel() { return savePromptPanel; }

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
        audioSource = FindFirstObjectByType<AudioSource>();
        if (GameStateManager.Instance.CurrentState is PlayingState)
        {
            GameStateManager.Instance.SetState(new GameStateManagement.PauseState(this));
        } else if(GameStateManager.Instance.CurrentState is not MainMenuState)
        {
            GameStateManager.Instance.SetState(new PlayingState());
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
    }

    public void CloseSettings()
    {
        settingsMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    public void CloseSavePrompt()
    {
        savePromptPanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        savePromptPanel.SetActive(true);
    }
}
