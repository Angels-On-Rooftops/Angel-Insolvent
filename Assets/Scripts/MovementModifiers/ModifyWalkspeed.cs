using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyWalkspeed : MonoBehaviour
{
    [SerializeField]
    AnimationCurve MultiplierOverTime;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out CharacterMovement movement))
        {
            return;
        }

        movement.WalkspeedMiddleware = WalkspeedMiddleware.MultiplyOverTime(MultiplierOverTime);
    }
}
