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
    float SpeedLimit;

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
            { "FacingVectorMiddleware", FacingMiddleware.FaceMovementDirection(Movement) },
        };

    public List<string> HoldFromPreviousState => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool hitWall,
        landed,
        pushedActionButton;
    readonly Maid StateMaid = new();
    bool doneAccelerating = false;

    public void TransitionedTo()
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);

        doneAccelerating = false;
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
        while (Movement.WalkSpeed < SpeedLimit)
        {
            Movement.WalkSpeed += AccelerationCurve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doneAccelerating = true;
    }
}
