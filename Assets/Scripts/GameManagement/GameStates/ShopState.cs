using System.Collections;
using System.Collections.Generic;
using GameStateManagement;
using UnityEngine;

namespace GameStateManagement
{
    public class ShopState : IGameState
    {
        private ShopController shopController;

        public ShopState(ShopController sControl)
        {
            shopController = sControl;
        }

        public void EnterState(IGameState previousState)
        {
            shopController.GetShopInventoryPanel().SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        public void ExitState()
        {
            shopController.GetShopInventoryPanel().SetActive(false);
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
