using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public IGameState CurrentState { get; private set; }

        [SerializeField]
        private string MainMenuSceneName = "MainMenu";

        private void Awake()
        {
            //Only one instance can exist
            if (Instance != null && Instance != this)
            {
                Debug.Log("Multiple instances of GameStateManager detected, destroying this one");
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            if (SceneManager.GetActiveScene().name == MainMenuSceneName)
            {
                SetState(new MainMenuState());
            }
            else
            {
                SetState(new PlayingState());
            }
        }

        public void SetState(IGameState newState)
        {
            if (CurrentState == newState)
            {
                return;
            }
            else
            {
                IGameState previousState = CurrentState;
                CurrentState = newState;

                if (previousState != null)
                {
                    previousState.ExitState();
                }

                newState.EnterState(previousState);
            }
        }
    }
}
