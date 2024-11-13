using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Testing
{
    public class TestDoor : MonoBehaviour
    {
        //Yes, this is a ridiculous way to make a door,
        //but since idk how we are handling doors yet (animation, pass through, etc.)
        //I'm not putting in the effort yet to code it correctly
        //The point of this is as a proof of concept of how InventoryChecker can be
        //used outside of collectables

        void OnCollisionStay(Collision collision)
        {
            HandleDoorBasedOnInventoryState();
        }

        private void OnTriggerStay(Collider other)
        {
            HandleDoorBasedOnInventoryState();
        }

        private void OnCollisionExit(Collision collision)
        {
            this.GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            this.GetComponent<Collider>().isTrigger = true;
        }

        private void HandleDoorBasedOnInventoryState()
        {
            InventoryChecker checker = this.GetComponent<InventoryChecker>();

            if (checker.InventoryStateMeetsCriteria())
            {
                this.GetComponent<Collider>().isTrigger = true;
            }
            else
            {
                this.GetComponent<Collider>().isTrigger = false;
            }
        }
    }
}
