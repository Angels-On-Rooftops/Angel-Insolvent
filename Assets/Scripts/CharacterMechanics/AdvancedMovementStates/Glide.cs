using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Glide : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float TerminalVelocity = 24;

    [SerializeField]
    float TurningSpeed = 5;

    [SerializeField]
    float DownSpeedContribCuttoff = 34;

    [SerializeField]
    float DownSpeedToHorizRate = 0.25f;

    [SerializeField]
    AnimationCurve UpSpeedDampAcceleration;

    void Start()
    {
        standardJumpHeight = Movement.JumpHeight;
        jumpBufferDur = Movement.JumpBufferTime;
    }

    public Dictionary<string, object> MovementProperties =>
        new()
        {
            { "DownwardTerminalVelocity", TerminalVelocity },
            {
                "MovementDirectionMiddleware",
                MovementMiddleware.NonZeroLimitedAdjust(Movement, TurningSpeed)
            },
        };
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            {
                AdvancedMovementState.Decelerating,
                Movement.IsOnStableGround() && !canHighJump || !Movement.Jump.IsPressed()
            },
            { AdvancedMovementState.Plunging, !Movement.IsOnStableGround() && pushedActionButton },
            { AdvancedMovementState.HighJumping, Movement.IsOnStableGround() && canHighJump }
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed", "VerticalSpeed" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    float HighJumpHeight => GetComponent<HighJump>().HighJumpHeight;

    readonly Maid StateMaid = new();
    bool canHighJump = false;
    float timeStarted;
    bool pushedActionButton = false;
    float standardJumpHeight;
    float jumpBufferDur;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        timeStarted = Time.time;

        if (Movement.WalkSpeed < DownSpeedContribCuttoff)
        {
            Movement.WalkSpeed = Mathf.Clamp(
                Movement.WalkSpeed + (DownSpeedToHorizRate * -Movement.VerticalSpeed),
                Movement.WalkSpeed,
                DownSpeedContribCuttoff
            );
        }

        Coroutine dampRoutine = null;
        if (Movement.VerticalSpeed > 0)
        {
            dampRoutine = StartCoroutine(DampVertical(UpSpeedDampAcceleration));
        }
        StateMaid.GiveTask(() =>
        {
            if (dampRoutine is not null)
            {
                StopCoroutine(dampRoutine);
            }
        });

        canHighJump = false;
        Coroutine highJumpCounter = null;
        if (fromState == AdvancedMovementState.Plunging)
        {
            canHighJump = true;
            Movement.JumpHeight = HighJumpHeight;
            highJumpCounter = StartCoroutine(HighJumpTimer());
        }
        StateMaid.GiveTask(() =>
        {
            if (highJumpCounter is not null)
            {
                StopCoroutine(highJumpCounter);
            }
        });

        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    IEnumerator HighJumpTimer()
    {
        while (Time.time - timeStarted <= jumpBufferDur)
        {
            yield return null;
        }
        canHighJump = false;
        Movement.JumpHeight = standardJumpHeight;
    }

    IEnumerator DampVertical(AnimationCurve curve)
    {
        float timeElapsed = 0;
        while (Movement.VerticalSpeed > 0)
        {
            Movement.VerticalSpeed += curve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        Movement.VerticalSpeed = 0;
    }
}
