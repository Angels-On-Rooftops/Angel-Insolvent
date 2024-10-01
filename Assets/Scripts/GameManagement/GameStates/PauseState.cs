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
            EscMenuController.getPauseMenuPanel().SetActive(true);
            Time.timeScale = 0f;
            EscMenuController.audioSource?.Pause();
        }

        public void ExitState()
        {
            if(EscMenuController.getSettingsMenuPanel().activeSelf)
            {
                EscMenuController.CloseSettings();
            }
            EscMenuController.getPauseMenuPanel().SetActive(false);
            Time.timeScale = 1f;
            EscMenuController.audioSource?.Play();
        }
    }
}