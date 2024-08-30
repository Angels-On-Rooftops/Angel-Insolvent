using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Items.Collectables
{
    [RequireComponent(typeof(Collectable))]
    public class AffectInventoryWhenCollect : MonoBehaviour
    {
        [SerializeField] private ItemData item;

        [Tooltip("Amount of the item")]
        [SerializeField] private int stackSize = 1;

        private Collectable collectable;

        private void Awake()
        {
            this.collectable = GetComponent<Collectable>();
        }

        void OnEnable()
        {
            this.collectable.OnCollection += AddToInventory;
        }

        void OnDisable()
        {
            this.collectable.OnCollection -= AddToInventory;
        }

        void AddToInventory(GameObject character)
        {
            character.GetComponent<AttachInventoryToCharacter>().InventorySystem.Add(this.item, this.stackSize);
        }
    }
}
