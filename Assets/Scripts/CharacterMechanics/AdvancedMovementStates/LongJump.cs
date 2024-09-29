using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJump : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    int PushOffSpeed = 24;

    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Decelerating, hitWall || Movement.IsOnStableGround() },
            { AdvancedMovementState.Diving, pushedActionButton },
        };
    public Dictionary<string, object> MovementProperties =>
        new()
        {
            { "WalkSpeed", PushOffSpeed },
            { "JumpHeight", GetComponent<Roll>().JumpOutHeight },
            { "MovementVectorMiddleware", MovementMiddleware.FullSpeedAhead(Movement, 2.5f) },
            { "FacingVectorMiddleware" , FacingMiddleware.FaceMovementDirection(Movement) },
        };

    public List<string> HoldFromPreviousState => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool hitWall,
        landed,
        pushedActionButton;
    readonly Maid StateMaid = new();

    public void TransitionedTo()
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }
}
