using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Can check the PlayerInventory based on SerializedFields.
    /// (Only fill out relevant SerializedFields--ones that are not edited will be ignored)
    /// </summary>
    public class InventoryChecker : MonoBehaviour, IChecker
    {
        [Tooltip("ItemData for equipment that needs to be currently equipped")]
        [SerializeField] ItemData mustHaveEquipped;

        [Tooltip("ItemData for equipment that must NOT be currently equipped")]
        [SerializeField] ItemData mustNotHaveEquipped;

        [Tooltip("ItemData and minimum stack size for items player must have in inventory")]
        [SerializeField] List<InventoryItem> mustHaveTheseItems;

        //The following may be useful if the criteria may be met by multiple different items:

        [Tooltip("Player must have at least one of the entries in this List of ItemData")]
        [SerializeField] List<ItemData> mustHaveAtLeastOneOfTheseItems;

        [Tooltip("Player must have currently equipped at least one of the entries in this List of ItemData")]
        [SerializeField] List<ItemData> mustHaveEquippedAtLeastOneOfTheseItems;

        /// <summary>
        /// Checks the PlayerInventory.
        /// Returns true if the PlayerInventory meets the criteria
        /// in SerializedFields for this 
        /// </summary>
        public bool InventoryStateMeetsCriteria()
        {
            if (this.mustHaveEquipped != null &&
                    PlayerInventory.Instance.CurrentlyEquippedItem != this.mustHaveEquipped)
            {
                return false;
            }

            if (this.mustNotHaveEquipped != null &&
                    PlayerInventory.Instance.CurrentlyEquippedItem == this.mustNotHaveEquipped)
            {
                return false;
            }

            if (!CheckMustHaveItems())
            {
                return false;
            }

            if (!CheckHaveAtLeastOneOfItems())
            {
                return false;
            }

            if(!CheckHaveEquippedAtLeastOneOfTheseItems())
            {
                return false;
            }

            return true;
        }

        private bool CheckMustHaveItems()
        {
            foreach (InventoryItem neededItem in this.mustHaveTheseItems)
            {
                if (!PlayerInventory.Instance.ItemDictionary.TryGetValue(neededItem.Data, out InventoryItem inventoryItem))
                {
                    return false;
                    
                }

                if (inventoryItem.StackSize < neededItem.StackSize)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckHaveAtLeastOneOfItems()
        {
            if (this.mustHaveAtLeastOneOfTheseItems.Count <= 0)
            {
                return true;
            }
            
            foreach (ItemData neededItem in this.mustHaveAtLeastOneOfTheseItems)
            {
                if (PlayerInventory.Instance.ItemDictionary.ContainsKey(neededItem))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckHaveEquippedAtLeastOneOfTheseItems()
        {
            if (this.mustHaveEquippedAtLeastOneOfTheseItems.Count <= 0)
            {
                return true;
            }

            return this.mustHaveEquippedAtLeastOneOfTheseItems.Contains
                            (PlayerInventory.Instance.CurrentlyEquippedItem);
        }

        public bool MeetsCriteria()
        {
            return InventoryStateMeetsCriteria();
        }
    }
}
