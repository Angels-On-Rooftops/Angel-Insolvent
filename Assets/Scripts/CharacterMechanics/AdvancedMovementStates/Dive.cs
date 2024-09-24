using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dive : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float FallAcceleration = 30;

    [SerializeField]
    float AdjustSpeed = 2;

    [SerializeField]
    float TerminalVelocity = 64;

    public Dictionary<AdvancedMovementState, bool> Transitions => new()
    {
        { AdvancedMovementState.Rolling, Movement.IsOnStableGround() },
        { AdvancedMovementState.None, hitWall },
    };
    public Dictionary<string, object> MovementProperties => new()
    {
        { "GravityMultiplier", FallAcceleration },
        { "WalkSpeed", AdjustSpeed },
        { "DownwardTerminalVelocity", TerminalVelocity },
        { "JumpHeight", 0 }
    };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool landed, hitWall;

    Maid StateMaid = new();

    public void TransitionedTo()
    {
        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }


}
