using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Items.Collectables;
using System;
using static UnityEditor.Progress;
using System.Linq;

namespace Inventory
{
    public class InventorySystem : IPersistableData
    {
        private Dictionary<ItemData, InventoryItem> itemDictionary; //to search by ItemData faster

        /// <summary>
        /// Public property to make accessing Inventory info easier,
        /// but DO NOT modify Dictionary entries outside of the InventorySystem class
        /// </summary>
        public Dictionary<ItemData, InventoryItem> ItemDictionary { get { return itemDictionary; } }

        public InventorySystem()
        {
            this.itemDictionary = new Dictionary<ItemData, InventoryItem>();
        }

        public virtual string PrintInventory()
        {
            string inventoryStr = "Inventory:\n";

            foreach (var itemPair in this.itemDictionary)
            {
                inventoryStr += itemPair.Key.itemName + ": " + itemPair.Value.StackSize + "\n";
            }

            return inventoryStr;
        }

        public virtual void Add(ItemData itemData, int amount = 1)
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
        }

        public virtual void Add(InventoryItem item)
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

        public virtual void Remove(ItemData itemData, int amount = 1)
        {
            if (this.itemDictionary.TryGetValue(itemData, out InventoryItem item))
            {
                item.RemoveFromStack(amount);

                if (item.StackSize <= 0)
                {
                    this.itemDictionary.Remove(item.Data);
                }
            }
            else
            {
                Debug.LogError("Attempt to Remove item from Inventory that does not exist.");
            }
        }

        public virtual void Remove(InventoryItem item)
        {
            this.Remove(item.Data, item.StackSize);
        }

        public void SaveData()
        {
            List<SerializableInventoryItem> items = new List<SerializableInventoryItem>();
            foreach(var item in this.itemDictionary.Values)
            {
                items.Add(new SerializableInventoryItem(item.Data.itemName, item.StackSize));
            }

            DataPersistenceManager.Instance.SaveData(new SerializableInventory(items));
        }
        public void LoadData()
        {
            SerializableInventory deserializedInventory = DataPersistenceManager.Instance.LoadData(typeof(SerializableInventory)) as SerializableInventory;
            ItemData[] allItems = Resources.LoadAll<ItemData>("");

            foreach(var deserializedItem in deserializedInventory.Inventory)
            {
                IEnumerable<ItemData> itemToLoad = from itemData in allItems
                                                   where itemData.itemName == deserializedItem.itemName
                                                   select itemData;
                if (itemToLoad.Any())
                {
                    this.Add(itemToLoad.FirstOrDefault(), deserializedItem.stackSize);
                }
            }
        }
    }

    [Serializable]
    public class SerializableInventory
    {
        public List<SerializableInventoryItem> Inventory;
        public SerializableInventory(List<SerializableInventoryItem> items)
        {
            Inventory = items;
        }
    }

    [Serializable]
    public class SerializableInventoryItem
    {
        public string itemName;
        public int stackSize;
        public SerializableInventoryItem(string _itemName, int _stackSize)
        {
            this.itemName = _itemName;
            this.stackSize = _stackSize;
        }
    }
}
