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
            //Subscribe save/load actions
            DataPersistenceManager.Instance.onSaveTriggered += inventorySystem.SaveData;
            DataPersistenceManager.Instance.onLoadTriggered += inventorySystem.LoadData;
        }

        private void OnDisable()
        {
            //Unsubscribe save/load actions
            DataPersistenceManager.Instance.onSaveTriggered -= inventorySystem.SaveData;
            DataPersistenceManager.Instance.onLoadTriggered -= inventorySystem.LoadData;
        }

        private void Start()
        {
            //Load inventory
            DataPersistenceManager.Instance.LoadGame(new UnityEngine.InputSystem.InputAction.CallbackContext());
        }

        /// <summary>
        /// Only use this Property after Awake()
        /// </summary>
        public InventorySystem InventorySystem { get { return this.inventorySystem; } }
    }
}
