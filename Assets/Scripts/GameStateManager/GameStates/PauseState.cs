using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStateManagement
{
    public class PauseState : MonoBehaviour, IGameState
    {
        private EscMenuController EscMenuController;

        public PauseState(EscMenuController escMenuController)
        {
            EscMenuController = escMenuController;
        }

        public void EnterState()
        {
            EscMenuController.pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ExitState()
        {
            if(EscMenuController.settingsMenuPanel.activeSelf)
            {
                EscMenuController.CloseSettings();
            }
            EscMenuController.pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}