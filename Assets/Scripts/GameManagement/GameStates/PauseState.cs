using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            characterCamera.CanOrbit = false;
            EscMenuController.getPauseMenuPanel().SetActive(true);
            Time.timeScale = 0f;
            if (EscMenuController.audioSource != null) EscMenuController.audioSource.Pause();
        }

        public void ExitState()
        {
            var characterCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterCamera>();
            characterCamera.CanOrbit = true;
            if (EscMenuController.getSettingsMenuPanel().activeSelf)
            {
                EscMenuController.CloseSettings();
            }
            EscMenuController.getPauseMenuPanel().SetActive(false);
            Time.timeScale = 1f;
            if (EscMenuController.audioSource != null) EscMenuController.audioSource.Play();
        }
    }
}