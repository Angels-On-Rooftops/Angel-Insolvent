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
        [SerializeField] ItemData data;
        [Tooltip("Amount of the item")]
        [SerializeField] int stackSize;

        public ItemData Data { get { return data; } }

        public int StackSize { get { return stackSize; } }

        public InventoryItem(ItemData itemData, int amount = 1)
        {
            this.data = itemData;
            AddToStack(amount);
        }

        public void AddToStack(int amount = 1)
        {
            this.stackSize += amount;
        }

        public void RemoveFromStack(int amount = 1)
        {
            if (this.StackSize > 0)
            {
                this.stackSize -= amount;
            }
            else
            {
                Debug.LogError("Attempt to Remove from Stack of " + this.Data.itemName + "with Size of " + this.StackSize);
            }
        }
    }
}

