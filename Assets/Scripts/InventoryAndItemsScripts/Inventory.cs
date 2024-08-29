using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Items.Collectables;

namespace Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        private Dictionary<ItemData, InventoryItem> itemDictionary; //to search by ItemData faster
        //private List<InventoryItem> inventoryItems;

        private void Awake()
        {
            this.itemDictionary = new Dictionary<ItemData, InventoryItem>();
            //this.inventoryItems = new List<InventoryItem>();
            Debug.Log("Start: " + PrintInventory());
        }

        private string PrintInventory()
        {
            string inventoryStr = "Inventory:\n";

            foreach (var itemPair in this.itemDictionary)
            {
                string itemPairStr = itemPair.Key.itemName + ": " + itemPair.Value.StackSize + "\n";
                inventoryStr += itemPairStr;
            }

            return inventoryStr;
        }

        public void Add(ItemData itemData, int amount = 1)
        {
            if (this.itemDictionary.TryGetValue(itemData, out InventoryItem item))
            {
                item.AddToStack(amount);
            }
            else
            {
                InventoryItem newItem = new InventoryItem(itemData, amount);
                this.itemDictionary.Add(itemData, newItem);  
            }

            Debug.Log(PrintInventory());
        }

        public void Add(InventoryItem item)
        {
            if (this.itemDictionary.TryGetValue(item.Data, out InventoryItem itemFromDict))
            {
                itemFromDict.AddToStack(item.StackSize);
            }
            else
            {
                this.itemDictionary.Add(item.Data, item);  
            }
        }

        public void Remove(ItemData itemData, int amount = 1)
        {
            if (this.itemDictionary.TryGetValue(itemData, out InventoryItem item))
            {
                item.RemoveFromStack(amount);

                if (item.StackSize == 0)
                {
                    this.itemDictionary.Remove(item.Data);
                }
            }
            else
            {
                Debug.LogError("Attempt to Remove item from Inventory that does not exist.");
            }
        }

        public void Remove(InventoryItem item)
        {
            if (this.itemDictionary.TryGetValue(item.Data, out InventoryItem itemFromDict))
            {
                itemFromDict.RemoveFromStack(item.StackSize);

                if (itemFromDict.StackSize == 0)
                {
                    this.itemDictionary.Remove(item.Data);
                }
            }
            else
            {
                Debug.LogError("Attempt to Remove item from Inventory that does not exist.");  
            }
        }
    }
}
