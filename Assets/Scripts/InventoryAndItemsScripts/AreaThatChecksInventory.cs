using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [RequireComponent(typeof(InventoryChecker))]
    [RequireComponent(typeof(Collider))]
    public class AreaThatChecksInventory : MonoBehaviour
    {
        public event Action<GameObject> FailedInventoryCheck;
        public event Action<GameObject> PassedInventoryCheck;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                InventoryChecker inventoryChecker = GetComponent<InventoryChecker>();

                if (inventoryChecker.InventoryStateMeetsCriteria())
                {
                    if (PassedInventoryCheck != null)
                    {
                        PassedInventoryCheck(other.gameObject);
                    }
                }
                else
                {
                    if (FailedInventoryCheck != null)
                    {
                        FailedInventoryCheck(other.gameObject);
                    }
                }
            }
        }
    }
}
