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

    //converts falling speed -> horizontal speed in glide
    [SerializeField]
    AnimationCurve FallSpeedCurve;

    [SerializeField]
    AnimationCurve GlideSpeedTransitionCurve;

    [SerializeField]
    AnimationCurve TerminalVelocityTransitionCurve;

    [SerializeField]
    float MaxGlideSpeed = 34;

    [SerializeField]
    AnimationCurve UpSpeedDampAcceleration;

    void Start()
    {
        standardJumpHeight = Movement.JumpHeight;
    }

    public Dictionary<string, object> MovementProperties =>
        new()
        {
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

    public List<string> HoldFromPreviousState =>
        new() { "WalkSpeed", "VerticalSpeed", "DownwardTerminalVelocity" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    float JumpBufferTime => GetComponent<CharacterMovement>().JumpBufferTime;
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    float HighJumpHeight => GetComponent<HighJump>().HighJumpHeight;

    readonly Maid StateMaid = new();
    bool canHighJump = false;
    bool pushedActionButton = false;
    float standardJumpHeight;
    float glideSpeedTarget;
    float initialGlideSpeed;
    float initialTerminalVelocity;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        initialGlideSpeed = Movement.WalkSpeed;
        initialTerminalVelocity = Mathf.Max(-Movement.VerticalSpeed, TerminalVelocity);
        glideSpeedTarget = Mathf.Clamp(
            FallSpeedCurve.Evaluate(-Movement.VerticalSpeed),
            Movement.WalkSpeed,
            MaxGlideSpeed
        );

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

        Coroutine glideTransitionRoutine = StartCoroutine(TransitionToGlide());
        StateMaid.GiveTask(() =>
        {
            if (glideTransitionRoutine is not null)
            {
                StopCoroutine(glideTransitionRoutine);
            }
        });

        canHighJump = false;
        Coroutine highJumpCounter = null;
        if (fromState == AdvancedMovementState.Plunging)
        {
            canHighJump = true;
            Movement.JumpHeight = HighJumpHeight;
            highJumpCounter = StartCoroutine(
                CoroutineUtil.DoActionAfterTime(
                    () =>
                    {
                        canHighJump = false;
                        Movement.JumpHeight = standardJumpHeight;
                    },
                    JumpBufferTime
                )
            );
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

    IEnumerator TransitionToGlide()
    {
        float timeElapsed = 0;
        while (
            TerminalVelocityTransitionCurve.keys[TerminalVelocityTransitionCurve.length - 1].time
            > timeElapsed
        )
        {
            Movement.WalkSpeed = Mathf.Lerp(
                initialGlideSpeed,
                glideSpeedTarget,
                GlideSpeedTransitionCurve.Evaluate(timeElapsed)
            );
            Movement.DownwardTerminalVelocity = Mathf.Lerp(
                initialTerminalVelocity,
                TerminalVelocity,
                TerminalVelocityTransitionCurve.Evaluate(timeElapsed)
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
