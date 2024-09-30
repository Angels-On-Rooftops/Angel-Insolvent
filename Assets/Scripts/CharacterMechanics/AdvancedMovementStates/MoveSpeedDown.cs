using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedDown : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    AnimationCurve AccelerationCurve;

    float defaultSpeed;

    void Start()
    {
        defaultSpeed = Movement.WalkSpeed;
    }

    public Dictionary<string, object> MovementProperties =>
        new() { { "MovementVectorMiddleware", MovementMiddleware.MoveInLookDir(Movement) }, };
    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.Rolling, pushedActionButton && Movement.IsOnGround() },
            { AdvancedMovementState.Plunging, pushedActionButton && !Movement.IsOnGround() },
            { AdvancedMovementState.MoveStarting, movementStarted },
            { AdvancedMovementState.None, rampDownEnded }
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    readonly Maid StateMaid = new();

    bool pushedActionButton = false;
    bool movementStarted = false;
    bool rampDownEnded = false;

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        if (Movement.WalkSpeed > defaultSpeed)
        {
            Movement.WalkSpeed = defaultSpeed;
        }

        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        movementStarted = false;
        StateMaid.GiveEvent(Movement, "WalkStartRequested", () => movementStarted = true);

        rampDownEnded = false;
        Coroutine rampDownRoutine = StartCoroutine(RampDown(AccelerationCurve));
        StateMaid.GiveTask(() =>
        {
            if (rampDownRoutine is not null)
            {
                StopCoroutine(rampDownRoutine);
            }
        });
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    IEnumerator RampDown(AnimationCurve curve)
    {
        float timeElapsed = 0;
        while (Movement.WalkSpeed > 0)
        {
            Movement.WalkSpeed += curve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        rampDownEnded = true;
        Movement.WalkSpeed = 0;
    }
}
