using System;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float RollingSpeed = 24;

    [SerializeField]
    float RollDuration = 1;

    [SerializeField]
    float JumpOutHeight = 2;

    [SerializeField]
    Collider RollingCollider;

    public Dictionary<AdvancedMovementState, bool> Transitions => new()
    {
        { AdvancedMovementState.LongJumping, pushedJumpButton },
        { AdvancedMovementState.Diving, pushedActionButton && !Movement.IsOnGround() },
        { AdvancedMovementState.None, IsRollOver() || hitWall },
    };

    public Dictionary<string, object> MovementProperties => new()
    {
        { "WalkSpeed", RollingSpeed },
        { "JumpHeight", JumpOutHeight },
    };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedJumpButton = false;
    bool pushedActionButton = false;
    float timeStarted;
    bool hitWall = false;

    public void TransitionedTo()
    {
        timeStarted = Time.time;

        pushedJumpButton = false;
        StateMaid.GiveEvent(Movement, "JumpRequested", () => pushedJumpButton = true);

        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = false);
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    bool IsRollOver()
    {
        return Time.time - timeStarted >= RollDuration;
    }
}
