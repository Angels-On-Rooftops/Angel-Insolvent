using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedUp : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    AnimationCurve AccelerationCurve;

    float defaultSpeed;

    void Start()
    {
        defaultSpeed = Movement.WalkSpeed;
    }

    public Dictionary<string, object> MovementProperties => new() { };
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Rolling, pushedActionButton && Movement.IsOnGround() },
            { AdvancedMovementState.Plunging, pushedActionButton && !Movement.IsOnGround() },
            { AdvancedMovementState.MoveStopping, movementEnded },
            { AdvancedMovementState.None, rampUpEnded }
        };

    public List<string> HoldFromPreviousState => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedActionButton = false;
    bool movementEnded = false;
    bool rampUpEnded = false;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        Movement.WalkSpeed = 0;

        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        movementEnded = false;
        StateMaid.GiveEvent(Movement, "WalkStopRequested", () => movementEnded = true);

        rampUpEnded = false;
        Coroutine rampUpRoutine = StartCoroutine(RampUp(AccelerationCurve));
        StateMaid.GiveTask(() =>
        {
            if (rampUpRoutine is not null)
            {
                StopCoroutine(rampUpRoutine);
            }
        });
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    IEnumerator RampUp(AnimationCurve curve)
    {
        float timeElapsed = 0;
        while (Movement.WalkSpeed < defaultSpeed)
        {
            Movement.WalkSpeed += curve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        rampUpEnded = true;
        Movement.WalkSpeed = defaultSpeed;
    }
}
