using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plunge : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float FallAcceleration = 30;

    [SerializeField]
    float MinAdjustSpeed = 2;

    [SerializeField]
    float TerminalVelocity = 64;

    [SerializeField]
    float DownwardsBoost = 5;

    [SerializeField]
    AnimationCurve HorizAccelerationCurve;

    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Rolling, Movement.IsOnStableGround() },
            { AdvancedMovementState.None, hitWall },
            {
                AdvancedMovementState.Gliding,
                jumpedOffGround && !Movement.IsOnGround() && Movement.ExtraJumpsRemaining == 0
            },
        };
    public Dictionary<string, object> MovementProperties =>
        new()
        {
            { "GravityMultiplier", FallAcceleration },
            { "DownwardTerminalVelocity", TerminalVelocity },
            { "JumpHeight", 0 },
            { "VerticalState", VerticalMovementState.Falling },
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool hitWall;

    Maid StateMaid = new();

    bool jumpedOffGround = false;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);

        jumpedOffGround = false;
        StateMaid.GiveEvent(Movement, "JumpRequested", () => jumpedOffGround = true);

        Coroutine decelerateRoutine = StartCoroutine(Decelerate());
        StateMaid.GiveTask(() =>
        {
            if (decelerateRoutine is not null)
            {
                StopCoroutine(decelerateRoutine);
            }
        });

        Movement.VerticalSpeed -= DownwardsBoost;
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    IEnumerator Decelerate()
    {
        float timeElapsed = 0;
        while (Movement.WalkSpeed > MinAdjustSpeed)
        {
            if (Movement.RawMovementDirection == Vector3.zero)
            {
                Movement.WalkSpeed = MinAdjustSpeed;
                yield break;
            }
            Movement.WalkSpeed += HorizAccelerationCurve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Movement.WalkSpeed = MinAdjustSpeed;
    }
}
