using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStateManagement
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance { get; private set; }

        public IGameState CurrentState { get; private set; }

        private void Awake()
        {
            //Only one instance can exist
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
            } 
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            CurrentState = new MainMenuState();
            CurrentState.EnterState();
        }

        public void SetState(IGameState newState)
        {
            if(CurrentState == newState)
            {
                return;
            } else
            {
                CurrentState.ExitState();
                CurrentState = newState;
                newState.EnterState();
            }
        }
    }
}