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
            { "JumpHeight", GetComponent<HighJump>().HighJumpHeight },
        };
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            {
                AdvancedMovementState.Decelerating,
                Movement.IsOnStableGround() && !canHighJump || !Movement.Jump.IsPressed()
            },
            { AdvancedMovementState.Plunging, !Movement.IsOnStableGround() && pushedActionButton },
            { AdvancedMovementState.HighJumping, Movement.IsOnStableGround() && canHighJump },
        };

    public List<string> HoldFromPreviousState =>
        new() { "WalkSpeed", "VerticalSpeed", "DownwardTerminalVelocity" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    float JumpBufferTime => GetComponent<CharacterMovement>().JumpBufferTime;
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

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

        if (Movement.VerticalSpeed > 0)
        {
            StateMaid.GiveCoroutine(this, StartCoroutine(DampVertical(UpSpeedDampAcceleration)));
        }

        StateMaid.GiveCoroutine(this, StartCoroutine(TransitionToGlide()));

        if (fromState == AdvancedMovementState.Plunging)
        {
            StateMaid.GiveCoroutine(
                this,
                StartCoroutine(
                    CoroutineUtil.DoActionAfterTime(
                        () =>
                        {
                            Movement.JumpHeight = standardJumpHeight;
                        },
                        JumpBufferTime
                    )
                )
            );
        }

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
