using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("here");
        if (other.TryGetComponent(out PlayerRespawn respawn))
        {
            if (other.TryGetComponent(out CharacterController controller))
            {
                controller.enabled = false;
                respawn.respawnPlayer();
                controller.enabled = true;
            }
        }
    }
}
