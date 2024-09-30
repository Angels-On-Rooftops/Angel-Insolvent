using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJump : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    AnimationCurve AccelerationCurve;

    [SerializeField]
    float PushOffSpeed = 24;

    [SerializeField]
    float MaxSpeedFromLongJump;

    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Decelerating, hitWall || Movement.IsOnStableGround() },
            { AdvancedMovementState.Diving, pushedActionButton },
            {
                AdvancedMovementState.Gliding,
                jumpedOffGround && !Movement.IsOnStableGround() && Movement.ExtraJumpsRemaining == 0
            },
        };
    public Dictionary<string, object> MovementProperties =>
        new()
        {
            { "JumpHeight", GetComponent<Roll>().LongJumpHeight },
            { "MovementVectorMiddleware", MovementMiddleware.FullSpeedAhead(Movement, 2.5f) },
            { "FacingVectorMiddleware", FacingMiddleware.FaceMovementDirection(Movement) },
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool hitWall,
        landed,
        pushedActionButton;
    readonly Maid StateMaid = new();
    bool jumpedOffGround = false;

    public void TransitionedTo(AdvancedMovementState oldState)
    {
        if (Movement.WalkSpeed < PushOffSpeed)
        {
            Movement.WalkSpeed = PushOffSpeed;
        }

        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);

        jumpedOffGround = false;
        StateMaid.GiveEvent(Movement, "JumpRequested", () => jumpedOffGround = true);

        Coroutine accelerateRoutine = StartCoroutine(Accelerate());
        StateMaid.GiveTask(() =>
        {
            if (accelerateRoutine is not null)
            {
                StopCoroutine(accelerateRoutine);
            }
        });
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    IEnumerator Accelerate()
    {
        float timeElapsed = 0;
        while (Movement.WalkSpeed < MaxSpeedFromLongJump)
        {
            Movement.WalkSpeed += AccelerationCurve.Evaluate(timeElapsed) * Time.deltaTime;
            Movement.WalkSpeed = Mathf.Clamp(Movement.WalkSpeed, 0, MaxSpeedFromLongJump);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
