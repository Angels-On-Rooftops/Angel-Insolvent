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

    public GameObject pauseMenuPanel;
    public GameObject settingsMenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenuPanel.SetActive(false);
        settingsMenuPanel.SetActive(false);
        DontDestroyOnLoad(gameObject);
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
        if(GameStateManager.Instance.CurrentState is PlayingState)
        {
            GameStateManager.Instance.SetState(new GameStateManagement.PauseState(this));
        } else
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

    public void Quit()
    {
        GameStateManager.Instance.SetState(new MainMenuState());
    }
}
