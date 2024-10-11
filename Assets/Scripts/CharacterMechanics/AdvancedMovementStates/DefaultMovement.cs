using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMovement : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float StandardSpeed;

    public Dictionary<string, object> MovementProperties =>
        new()
        {
            { "WalkSpeed", StandardSpeed },
            { "DownwardTerminalVelocity", standardTerminalVelocity }
        };
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Rolling, pushedActionButton && Movement.IsOnGround() },
            { AdvancedMovementState.Plunging, pushedActionButton && !Movement.IsOnGround() },
            { AdvancedMovementState.MoveStarting, movementStarted },
            { AdvancedMovementState.MoveStopping, movementEnded },
            {
                AdvancedMovementState.Gliding,
                Movement.Jump.IsPressed()
                    && !Movement.IsOnStableGround()
                    && Movement.ExtraJumpsRemaining == 0
                    && Movement.VerticalSpeed < 0
            },
        };

    void Awake()
    {
        standardTerminalVelocity = Movement.DownwardTerminalVelocity;
    }

    public List<string> HoldFromPreviousState => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedActionButton = false;
    bool movementStarted = false;
    bool movementEnded = false;

    float standardTerminalVelocity;

    public void TransitionedTo(AdvancedMovementState fromState)
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
