using System;
using UnityEngine;

public class ImpulseOnTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The initial velocity to apply to the character when the trigger is touched.")]
    float InitialSpeed = 10;

    [SerializeField]
    [Tooltip("The direction of this transform from the trampoline's transform determines ")]
    Transform AimAt;

    event Action ImpulseApplied;

    private void Awake()
    {
        Debug.Assert(
            AimAt is not null,
            $"ImpulseOnTrigger must have an AimAt parameter on {gameObject.name}."
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out CharacterMovement movement))
        {
            Vector3 bounceDir = AimAt.position - transform.position;

            movement.GiveImpulse(
                bounceDir.normalized * InitialSpeed,
                bounceDir.magnitude / InitialSpeed / (1 - movement.ImpulseIntegral01),
                bounceDir.y > 0.01 ? VerticalMovementState.Jumping : VerticalMovementState.Falling
            );

            ImpulseApplied?.Invoke();
        }
    }
}
