using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using Items.Collectables;
using System;

namespace Inventory
{
    public class InventoryWithEquipment : InventorySystem
    {
        private List<ItemData> equipmentList;
        private ItemData currentlyEquippedItem = null; //can be changed to a list later if we want multiple equipped items
        private int currentlyEquippedItemIndex = -1;

        /// <summary>
        /// Public property to make accessing Equipment info easier,
        /// but DO NOT modify entries outside of the InventoryWithEquipment class
        /// </summary>
        public List<ItemData> EquipmentList { get { return equipmentList; } }

        /// <summary>
        /// null if nothing is currently equipped
        /// </summary>
        public ItemData CurrentlyEquippedItem { get { return currentlyEquippedItem; } }

        /// <summary>
        /// Index of the currently equipped item in the EquipmentList
        /// (-1 if nothing is currently equipped)
        /// </summary>
        public int CurrentlyEquippedItemIndex { get { return currentlyEquippedItemIndex; } }

        public InventoryWithEquipment() : base()
        {
            this.equipmentList = new List<ItemData>();
        }

        public virtual void UnEquipItem()
        {
            this.currentlyEquippedItem = null;
            this.currentlyEquippedItemIndex = -1;
        }

        public virtual void EquipItem(int equipmentListItemIndex)
        {
            this.currentlyEquippedItem = this.EquipmentList[equipmentListItemIndex];
            this.currentlyEquippedItemIndex = equipmentListItemIndex;
        }

        public virtual void EquipItem(ItemData equipmentListItem)
        {
            this.currentlyEquippedItemIndex = this.EquipmentList.IndexOf(equipmentListItem);
            this.currentlyEquippedItem = equipmentListItem;
        }

        /// <summary>
        /// If the equipment list has at least 1 item,
        /// will equip the next item in the list (looping around after hitting the end)
        /// (if there are 0 items, does nothing)
        /// </summary>
        public void EquipNext()
        {
            if (this.equipmentList.Count == 0)
            {
                return;
            }
            
            if (this.currentlyEquippedItemIndex == -1
                    || this.currentlyEquippedItemIndex == (this.equipmentList.Count - 1))
            {
                // there is nothing currently equipped,
                //  or the currently equipped item is the last one in the equipmentList,
                //  so equip the first item in the list
                EquipItem(0);
            }
            else
            {
                EquipItem(this.currentlyEquippedItemIndex + 1);
            } 
        }

        public override string PrintInventory()
        {
            string inventoryStr = base.PrintInventory() + "\nEquipment:\n";

            foreach (var item in this.equipmentList)
            {
                inventoryStr += item.itemName + "\n";
            }

            inventoryStr += "\nCurrently Equipped: ";

            if (this.currentlyEquippedItem != null)
            {
                inventoryStr += this.currentlyEquippedItem.itemName + " (Index: " + this.currentlyEquippedItemIndex + ")\n";
            }
            else
            {
                inventoryStr += "none\n";
            }

            return inventoryStr;
        }

        public override void Add(ItemData itemData, int amount = 1)
        {
            //check whether the item is a new equippable item
            if (itemData.isEquippable && !this.ItemDictionary.ContainsKey(itemData))
            {
                this.equipmentList.Add(itemData);
            }
            
            base.Add(itemData, amount);
        }

        public override void Add(InventoryItem item)
        {
            this.Add(item.Data, item.StackSize);
        }

        public override void Remove(ItemData itemData, int amount = 1)
        {
            //check whether the item is an equippable item currently in the inventory
            if (itemData.isEquippable && this.ItemDictionary.ContainsKey(itemData))
            {
                this.equipmentList.Remove(itemData);
            }

            base.Remove(itemData, amount); 
        }

        public override void Remove(InventoryItem item)
        {
            this.Remove(item.Data, item.StackSize);
        }
    }
}
