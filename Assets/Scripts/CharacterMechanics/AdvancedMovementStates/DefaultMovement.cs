using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMovement : MonoBehaviour, IAdvancedMovementStateSpec
{
    public Dictionary<string, object> MovementProperties => new();
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Rolling, pushedActionButton && Movement.IsOnGround() },
            { AdvancedMovementState.Diving, pushedActionButton && !Movement.IsOnGround() },
            { AdvancedMovementState.MoveStart, movementStarted },
            { AdvancedMovementState.MoveStop, movementEnded },
        };

    public List<string> HoldFromPreviousState => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedActionButton = false;
    bool movementStarted = false;
    bool movementEnded = false;

    public void TransitionedTo()
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        movementStarted = false;
        StateMaid.GiveEvent(Movement, "WalkStartRequested", () => movementStarted = true);

        movementEnded = false;
        StateMaid.GiveEvent(Movement, "WalkStopRequested", () => movementEnded = true);
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }
}
