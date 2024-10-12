using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameStateManagement
{
    public class PauseState : IGameState
    {
        private EscMenuController EscMenuController;

        public PauseState(EscMenuController escMenuController)
        {
            EscMenuController = escMenuController;
        }

        public void EnterState()
        {
            var characterCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterCamera>();
            if(characterCamera != null) characterCamera.CanOrbit = false;
            EscMenuController.getPauseMenuPanel().SetActive(true);
            Time.timeScale = 0f;
            if (EscMenuController.audioSource != null) EscMenuController.audioSource.Pause();
            EscMenuController.getPauseMenuPanel().GetComponentInChildren<Button>().Select();
        }

        public void ExitState()
        {
            var characterCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterCamera>();
            characterCamera.CanOrbit = true;

            if (EscMenuController.getSettingsMenuPanel().activeSelf)
            {
                EscMenuController.CloseSettings();
            }
            if(EscMenuController.getSavePromptPanel().activeSelf)
            {
                EscMenuController.CloseSavePrompt();
            }

            EscMenuController.getPauseMenuPanel().SetActive(false);
            Time.timeScale = 1f;
            if (EscMenuController.audioSource != null) EscMenuController.audioSource.Play();
        }
    }
}