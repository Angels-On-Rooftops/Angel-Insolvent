using System;
using System.Collections;
using UnityEngine;

public static class MovementMiddleware
{
    public static Func<Vector3, float, Vector3> FullSpeedAhead(
        CharacterMovement movement,
        float turningSpeed
    )
    {
        Vector3 movementDirection = Vector3.zero;

        return new Func<Vector3, float, Vector3>(
            (rawVector, dt) =>
            {
                // set initial direction on first iteration
                if (movementDirection.magnitude == 0)
                {
                    movementDirection = movement.RawFacingDirection;
                    return movementDirection;
                }

                // any time the player is holding a direction, bring the direction closer to that goal
                if (rawVector.magnitude > 0)
                {
                    movementDirection = Vector3
                        .Lerp(movementDirection, rawVector.normalized, dt * turningSpeed)
                        .normalized;
                }

                return movementDirection;
            }
        );
    }

    public static Func<Vector3, float, Vector3> MoveInLookDir(CharacterMovement movement)
    {
        return new Func<Vector3, float, Vector3>(
            (rawVector, dt) =>
            {
                return movement.RawFacingDirection;
            }
        );
    }
}
