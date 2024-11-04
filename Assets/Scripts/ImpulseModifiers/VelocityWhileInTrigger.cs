using System;
using UnityEngine;

public class VelocityWhileInTrigger : MonoBehaviour
{
    [SerializeField]
    float Velocity = 0;

    [SerializeField]
    Vector3 DirectionalOverride = Vector3.zero;

    [SerializeField]
    float PushTimeAfterExit = 0.3f;

    Vector3 pushDirection()
    {
        return DirectionalOverride.magnitude != 0 ? DirectionalOverride : transform.up;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovement movement))
        {
            movement.GiveImpulse(pushDirection() * Velocity, PushTimeAfterExit);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovement movement))
        {
            movement.GiveImpulse(pushDirection() * Velocity, 999);
        }
    }
}
