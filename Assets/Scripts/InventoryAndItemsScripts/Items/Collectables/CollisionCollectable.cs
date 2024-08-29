using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

namespace Items.Collectables
{
    [RequireComponent(typeof(Collider))]
    public class CollisionCollectable : Collectable
    {
        private bool hasBeenCollected = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!this.hasBeenCollected && (other.gameObject.GetComponent<InventorySystem>() != null))
            {
                this.hasBeenCollected = true; //Do not allow the Player to collect the Collectable multiple times
                StartCoroutine(CollectionRoutine(other.gameObject));
            }
        }
    }
}
