using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJump : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    int PushOffSpeed = 24;

    public Dictionary<AdvancedMovementState, bool> Transitions => new()
    {
        { AdvancedMovementState.None, hitWall || landed },
        { AdvancedMovementState.Diving, pushedActionButton },

    };
    public Dictionary<string, object> MovementProperties => new()
    {
        { "WalkSpeed", PushOffSpeed },
        { "JumpHeight", GetComponent<Roll>().JumpOutHeight },
    };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool hitWall, landed, pushedActionButton;
    readonly Maid StateMaid = new();

    public void TransitionedTo()
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

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
