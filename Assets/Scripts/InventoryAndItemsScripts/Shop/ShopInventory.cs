using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class ShopInventory : InventorySystem
    {
        private static ShopInventory instance = null;
        private static readonly object instanceLock = new object();

        ShopInventory() : base() { }

        public static ShopInventory Instance
        {
            get
            {
                lock(instanceLock) {
                    if (instance == null) {
                        instance = new ShopInventory();
                    }
                    return instance;
                }
            }
        }

        public event Action OnPurchase;

        public override void Add(ItemData itemData, int amount = 1)
        {
            base.Add(itemData, amount);
        }

        public override void Add(InventoryItem item)
        {
            base.Add(item);
        }

        public override void Remove(ItemData itemData, int amount = 1)
        {
            base.Remove(itemData, amount);
        }

        public override void Remove(InventoryItem item)
        {
            base.Remove(item);
        }

        public void PurchaseItem(InventoryItem item)
        {
            this.Remove(item);
        }
    }
}
