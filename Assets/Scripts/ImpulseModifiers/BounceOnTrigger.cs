using System;
using UnityEngine;

public class BounceOnTrigger : MonoBehaviour
{
    [Serializable]
    internal class DistanceOrTime
    {
        public float Time = 0;
        public float Distance = 0;
    }

    [SerializeField]
    float BounceVelocity = 0;

    [SerializeField]
    DistanceOrTime ChooseOne;

    [SerializeField]
    Vector3 DirectionalOverride = Vector3.zero;

    private void Awake()
    {
        Debug.Assert(
            ChooseOne.Time != 0 || ChooseOne.Distance != 0,
            $"One of Time or Distance must be non-zero! On {gameObject.name}"
        );

        Debug.Assert(
            ChooseOne.Time == 0 || ChooseOne.Distance == 0,
            $"Both Time and Distance are non-zero, this means time will be used. This is probably not intended. On {gameObject.name}"
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Assert(
            ChooseOne.Time != 0 || ChooseOne.Distance != 0,
            "One of Time or Distance must be non-zero!"
        );

        if (other.TryGetComponent(out CharacterMovement movement))
        {
            Vector3 bounceDir =
                DirectionalOverride.magnitude != 0 ? DirectionalOverride : transform.up;

            movement.GiveImpulse(
                bounceDir * BounceVelocity,
                ChooseOne.Time != 0 ? ChooseOne.Time : ChooseOne.Distance / BounceVelocity,
                bounceDir.y > 0.01 ? VerticalMovementState.Jumping : VerticalMovementState.Falling
            );
            // force a wall hit here since triggers aren't picked up by raycasts from the charatcer controller
            movement.ForceHitWall();

            GetComponent<AudioSource>().Play();
        }
    }
}
