using System.Collections;
using System.Collections.Generic;
using GameStateManagement;
using UnityEngine;

namespace GameStateManagement
{
    public class InventoryState : IGameState
    {
        private InventoryController invController;

        public InventoryState(InventoryController iControl)
        {
            invController = iControl;
        }

        public void EnterState(IGameState previousState)
        {
            invController.GetInventoryPanel().SetActive(true);
            Time.timeScale = 0f;
        }

        public void ExitState()
        {
            invController.GetInventoryPanel().SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
