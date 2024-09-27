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
    }

    private void OnEnable()
    {
        pauseAction.performed += MenuToggle;
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.performed -= MenuToggle;
        pauseAction.Disable();
    }

    public void MenuToggle(CallbackContext c)
    {
        if(pauseMenuPanel.activeSelf || settingsMenuPanel.activeSelf)
        {
            // Resume game
            if (settingsMenuPanel.activeSelf)
            {
                CloseSettings();
            }
            pauseMenuPanel.SetActive(false);
            PauseSystem.ResumeGame();
        }
        else
        {
            // check if something else is pausing the game.
            // if not, then open pause menu
            if (!PauseSystem.isPaused) { 
                pauseMenuPanel.SetActive(true);
                PauseSystem.PauseGame();
            } // otherwise do nothing
        }
    }

    public void OnResumeButtonClick()
    {
        MenuToggle(new CallbackContext());
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
