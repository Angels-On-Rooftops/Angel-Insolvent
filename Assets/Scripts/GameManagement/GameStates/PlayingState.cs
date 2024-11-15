using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.RoomSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameStateManagement
{
    public class PlayingState : IGameState
    {
        public void EnterState(IGameState previousState)
        {
            if (previousState is MainMenuState)
            {
                Debug.Log("Playing!");
                RoomSystem.LoadLastGameplayRoom();
            }
        }

        public void ExitState() { }
    }
}
