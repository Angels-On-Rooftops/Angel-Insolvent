using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
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

        public void EnterState()
        {
            shopController.GetShopInventoryPanel().SetActive(true);
            Time.timeScale = 0f;
        }

        public void ExitState()
        {
            shopController.GetShopInventoryPanel().SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
