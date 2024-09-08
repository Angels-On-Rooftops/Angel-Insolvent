using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class PlayingState : MonoBehaviour, IGameState
    {
        private Scene scene;
        private bool newScene;
        private EscMenuController EscMenuController;

        public PlayingState(EscMenuController escMenuController)
        {
            this.scene = SceneManager.GetActiveScene();
            newScene = false;
            EscMenuController = escMenuController;
        }
        public PlayingState(EscMenuController escMenuController, Scene scene)
        {
            this.scene = scene;
            newScene = true;
            EscMenuController = escMenuController;
        }

        public void EnterState()
        {
            if (newScene)
            {
                SceneManager.LoadScene(scene.path);
            }
        }

        public void ExitState()
        {

        }
    }
}