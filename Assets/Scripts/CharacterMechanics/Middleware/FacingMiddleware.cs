using System;
using System.Collections;
using UnityEngine;

public static class FacingMiddleware
{
    public static Func<Vector3, float, Vector3> UpdateOnlyWhenMoving(CharacterMovement movement)
    {
        return (v, dt) =>
        {
            if (movement.MovementDirection.magnitude == 0)
            {
                return movement.FacingDirection;
            }

            return Quaternion.LookRotation(
                    -movement.ForwardMovementDirectionFromCamera(),
                    Vector3.up
                ) * movement.RawFacingDirection;
        };
    }

    public static Func<Vector3, float, Vector3> FaceMovementDirection(CharacterMovement movement)
    {
        return (v, dt) =>
        {
            return movement.MovementDirection.normalized;
        };
    }
}
