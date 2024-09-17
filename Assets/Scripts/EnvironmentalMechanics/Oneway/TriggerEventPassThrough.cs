using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventPassThrough : MonoBehaviour
{
    public event Action PlayerEnter;
    public event Action PlayerExit;

    bool IsPlayer(Collider maybePlayer)
    {
        return maybePlayer.TryGetComponent<CharacterMovement>(out _);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            PlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            PlayerExit?.Invoke();
        }
    }
}
