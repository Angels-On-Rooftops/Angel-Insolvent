using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Items;

namespace Inventory
{
    /// <summary>
    /// Singleton Inventory to make working with the UI easier
    /// </summary>
    public class PlayerInventory : InventorySystem
    {
        private static PlayerInventory instance = null;
        private static readonly object instanceLock = new object(); //thread-safe for co-routines

        PlayerInventory()
        {
            InitializeInventory();
        }

        public static PlayerInventory Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new PlayerInventory();
                    }
                    return instance;
                }
            }
        }

        public event Action OnInventoryUpdate;

        public override void Add(ItemData itemData, int amount = 1)
        {
            base.Add(itemData, amount);

            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }
        }

        public override void Add(InventoryItem item)
        {
            base.Add(item);

            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }
        }
            
        public override void Remove(ItemData itemData, int amount = 1)
        {
            base.Remove(itemData, amount);

            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }
        }

        public override void Remove(InventoryItem item)
        {
            base.Remove(item);

            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }
        }
    }
}

