using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using System;

namespace Items.Collectables
{
    [RequireComponent(typeof(Collider))]
    public class CollisionCollectable : Collectable
    {
        private bool hasBeenCollected = false;

        bool HasInventory(GameObject potentialInventoryHaver)
        {
            return potentialInventoryHaver.GetComponent<AttachInventoryToCharacter>() != null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hasBeenCollected && HasInventory(other.gameObject))
            {
                hasBeenCollected = true; //Do not allow the Player to collect the Collectable multiple times
                StartCoroutine(CollectionRoutine(other.gameObject));
            }
        }
    }
}
