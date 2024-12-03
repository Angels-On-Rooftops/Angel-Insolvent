using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class MainMenuState : IGameState
    {
        public void EnterState(IGameState previousState)
        {
            // TODO change to using room manager
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
            {
                var characterCamera = GameObject
                .FindGameObjectWithTag("PlayerCamera")?
                .GetComponent<CharacterCamera>();
                if (characterCamera != null)
                {
                    characterCamera.CanOrbit = false;
                    Cursor.lockState = CursorLockMode.None;
                }
                SceneManager.LoadScene("MainMenu");
            }
            Debug.Log("entered main menu state");
        }

        public void ExitState()
        {
            Debug.Log("exit main menu state");
        }
    }
}
