using System;
using System.Collections.Generic;
using UnityEngine;

public class HighJump : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float HighJumpStrafeSpeed;

    public Dictionary<string, object> MovementProperties =>
        new() { { "WalkSpeed", HighJumpStrafeSpeed } };
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Plunging, pushedActionButton && !Movement.IsOnGround() },
            { AdvancedMovementState.MoveStarting, movementStarted },
            { AdvancedMovementState.MoveStopping, movementEnded },
            { AdvancedMovementState.Gliding, jumpedOffGround },
            { AdvancedMovementState.None, Movement.IsOnStableGround() }
        };

    public List<string> HoldFromPreviousState => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedActionButton = false;
    bool movementStarted = false;
    bool movementEnded = false;
    bool jumpedOffGround = false;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        movementStarted = false;
        StateMaid.GiveEvent(Movement, "WalkStartRequested", () => movementStarted = true);

        movementEnded = false;
        StateMaid.GiveEvent(Movement, "WalkStopRequested", () => movementEnded = true);

        jumpedOffGround = false;
        StateMaid.GiveEvent(
            Movement,
            "JumpRequested",
            () =>
            {
                if (Movement.VerticalState != VerticalMovementState.Grounded)
                {
                    jumpedOffGround = true;
                }
            }
        );
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

}