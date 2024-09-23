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
        { AdvancedMovementState.Rolling, landed },
        { AdvancedMovementState.None, hitWall },
    };
    public Dictionary<string, object> MovementProperties => new()
    {
        { "GravityMultiplier", FallAcceleration },
        { "WalkSpeed", AdjustSpeed },
        { "DownwardTerminalVelocity", TerminalVelocity }
    };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool landed, hitWall;

    Maid StateMaid = new();

    public void TransitionedTo()
    {
        landed = false;
        StateMaid.GiveEvent(Movement, "Landed", () => landed = true);

        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }


}
