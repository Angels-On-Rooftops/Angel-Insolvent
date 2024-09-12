using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Attaches an Inventory to a Character
    /// (since having the Inventory be a MonoBehaviour caused problems)
    /// </summary>
    public class AttachInventoryToCharacter : MonoBehaviour
    {
        [Tooltip("Set to true iff using the PlayerInventory Singleton")]
        [SerializeField] private bool usePlayerInventory;

        private InventorySystem inventorySystem;

        private void Awake()
        {
            if (this.usePlayerInventory)
            {
                this.inventorySystem = PlayerInventory.Instance;
            }
            else
            {
                this.inventorySystem = new InventorySystem();
            }
        }

        private void OnEnable()
        {
            //Subscribe save action
            DataPersistenceManager.Instance.onSaveTriggered += inventorySystem.SaveData;
        }

        private void OnDisable()
        {
            //Unsubscribe save action
            DataPersistenceManager.Instance.onSaveTriggered -= inventorySystem.SaveData;
        }

        /// <summary>
        /// Only use this Property after Awake()
        /// </summary>
        public InventorySystem InventorySystem { get { return this.inventorySystem; } }
    }
}
