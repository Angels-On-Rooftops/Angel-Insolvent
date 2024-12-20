using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerRespawn respawn))
        {
            respawn.RespawnPlayer();
        }
    }
}
