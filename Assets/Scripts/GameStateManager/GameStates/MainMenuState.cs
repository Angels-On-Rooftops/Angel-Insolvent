using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class MainMenuState : MonoBehaviour, IGameState
    {
        public void EnterState()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void ExitState()
        {
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    }
}