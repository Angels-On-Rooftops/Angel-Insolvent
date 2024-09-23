using System;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    public float RollingSpeed = 24;

    [SerializeField]
    public float RollDuration = 1;

    [SerializeField]
    public float JumpOutHeight = 2;

    [SerializeField]
    float ColliderHeight = 1;

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
        //{ "MiddleWare", }
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

        StateMaid.GiveTask(AdvancedMovement.SetColliderHeight(ColliderHeight));
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    bool IsRollOver()
    {
        return Time.time - timeStarted >= RollDuration;
    }

    void LateUpdate()
    {
        pushedActionButton = false;
    }
}
