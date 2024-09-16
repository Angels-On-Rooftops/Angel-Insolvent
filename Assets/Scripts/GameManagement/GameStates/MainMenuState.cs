using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class MainMenuState : IGameState
    {
        public void EnterState()
        {
            if(SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        public void ExitState()
        {
            Debug.Log("exit main menu state");
        }
    }
}