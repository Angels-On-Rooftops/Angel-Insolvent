using System;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMovement : MonoBehaviour, IAdvancedMovementStateSpec
{
    public Dictionary<string, object> MovementProperties => new();
    public Dictionary<AdvancedMovementState, bool> Transitions => new()
    {
        { AdvancedMovementState.Rolling, pushedActionButton && Movement.IsOnGround() },
        { AdvancedMovementState.Diving, pushedActionButton && !Movement.IsOnGround() },
    };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedActionButton = false;

    public void TransitionedTo()
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }
}