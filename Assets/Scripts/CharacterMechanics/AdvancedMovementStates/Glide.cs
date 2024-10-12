using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glide : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float TerminalVelocity = 24;

    [SerializeField]
    float DownSpeedContribCuttoff = 34;

    [SerializeField]
    float DownSpeedToHorizRate = 0.25f;

    [SerializeField]
    AnimationCurve UpSpeedDampAcceleration;

    float minWalkSpeed;

    void Start()
    {
        minWalkSpeed = Movement.WalkSpeed;
    }

    public Dictionary<string, object> MovementProperties =>
        new() { { "DownwardTerminalVelocity", TerminalVelocity }, };
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Decelerating, Movement.IsOnStableGround() || jumpedOffGround },
            { AdvancedMovementState.Plunging, !Movement.IsOnStableGround() && pushedActionButton },
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed", "VerticalSpeed" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedActionButton = false;
    bool jumpedOffGround = false;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        if (Movement.WalkSpeed < minWalkSpeed)
        {
            Movement.WalkSpeed = minWalkSpeed;
        }
        if (Movement.WalkSpeed < DownSpeedContribCuttoff)
        {
            Movement.WalkSpeed = Mathf.Clamp(
                Movement.WalkSpeed + (DownSpeedToHorizRate * -Movement.VerticalSpeed),
                0,
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

        jumpedOffGround = false;
        StateMaid.GiveEvent(
            Movement,
            "JumpRequested",
            () =>
            {
                if (Movement.VerticalState != VerticalMovementState.Grounded)
                {
                    jumpedOffGround = true;
                }
            }
        );

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
}
