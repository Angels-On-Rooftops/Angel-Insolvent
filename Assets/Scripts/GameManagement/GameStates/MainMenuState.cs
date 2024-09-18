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
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("MainMenu"))
            {
                SceneManager.LoadScene("MainMenu New");
            }
            Debug.Log("entered main menu state");
        }

        public void ExitState()
        {
            Debug.Log("exit main menu state");
        }
    }
}
