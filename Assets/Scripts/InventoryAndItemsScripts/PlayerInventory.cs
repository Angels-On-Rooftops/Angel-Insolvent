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
    public class PlayerInventory : InventoryWithEquipment
    {
        private static PlayerInventory instance = null;
        private static readonly object instanceLock = new object(); //thread-safe for co-routines
        private int playerCoins = 0;

        PlayerInventory() : base() { }

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
        public event Action OnEquippedChange;

        public void AddCoins(int amount)
        {
            playerCoins += amount;
        }

        public void RemoveCoins(int amount)
        {
            playerCoins -= amount;
        }

        public int GetCoins() { return playerCoins; }

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
            this.Add(item.Data, item.StackSize);
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
            this.Remove(item.Data, item.StackSize);
        }

        //InventoryWithEquipment Methods:

        public override void UnEquipItem()
        {
            base.UnEquipItem();

            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }

            if (OnEquippedChange != null)
            {
                OnEquippedChange();
            }
        }

        public override void EquipItem(int equipmentListItemIndex)
        {
            base.EquipItem(equipmentListItemIndex);

            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }

            if (OnEquippedChange != null)
            {
                OnEquippedChange();
            }
        }

        public override void EquipItem(ItemData equipmentListItem)
        {
            base.EquipItem(equipmentListItem);

            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }

            if (OnEquippedChange != null)
            {
                OnEquippedChange();
            }
        }
    }
}

