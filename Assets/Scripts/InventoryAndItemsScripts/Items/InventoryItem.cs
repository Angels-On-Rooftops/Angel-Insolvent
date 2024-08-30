using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// Allows for the manipulation of the item's StackSize, and has ItemData
    /// </summary>
    [Serializable]
    public class InventoryItem
    {
        public ItemData Data { get; private set; }

        [Tooltip("Amount of the item")]
        public int StackSize { get; private set; }

        public InventoryItem(ItemData itemData, int amount = 1)
        {
            this.Data = itemData;
            AddToStack(amount);
        }

        public void AddToStack(int amount = 1)
        {
            this.StackSize += amount;
        }

        public void RemoveFromStack(int amount = 1)
        {
            if (this.StackSize > 0)
            {
                this.StackSize -= amount;
            }
            else
            {
                Debug.LogError("Attempt to Remove from Stack of " + this.Data.itemName + "with Size of " + this.StackSize);
            }
        }
    }
}

