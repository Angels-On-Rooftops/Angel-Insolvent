using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class PlayingState : IGameState
    {
        private string sceneName;
        private bool newScene;

        public PlayingState()
        {
            this.sceneName = SceneManager.GetActiveScene().name;
            newScene = false;
        }
        public PlayingState(string sceneName)
        {
            this.sceneName = sceneName;
            newScene = true;
        }

        public void EnterState()
        {
            if (newScene)
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        public void ExitState()
        {

        }
    }
}