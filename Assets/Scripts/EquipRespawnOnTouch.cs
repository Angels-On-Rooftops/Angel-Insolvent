using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipRespawnOnTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("here");
        if (other.TryGetComponent(out PlayerRespawn respawn))
        {
            if (other.TryGetComponent(out CharacterController controller))
            {
                if (
                    this.TryGetComponent(out InventoryChecker c) && !c.InventoryStateMeetsCriteria()
                )
                    controller.enabled = false;
                respawn.respawnPlayer();
                controller.enabled = true;
            }
        }
    }
}
