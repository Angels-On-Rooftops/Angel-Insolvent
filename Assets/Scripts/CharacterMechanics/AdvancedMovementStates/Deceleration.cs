using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deceleration : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    AnimationCurve AccelerationCurve;

    [SerializeField]
    AnimationCurve FastAccelerationCurve;

    void Start()
    {
        defaultWalkSpeed = Movement.WalkSpeed;
    }

    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.None, doneDecelerating },
            { AdvancedMovementState.MoveStopping, controlLifted },
            { AdvancedMovementState.Rolling, pushedActionButton && Movement.IsOnGround() },
            { AdvancedMovementState.Diving, pushedActionButton && !Movement.IsOnGround() },
            { AdvancedMovementState.Gliding, jumpedOffGround },
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed" };

    public Dictionary<string, object> MovementProperties => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    bool pushedActionButton = false;
    bool doneDecelerating = false;
    bool controlLifted = false;
    bool jumpedOffGround = false;
    float defaultWalkSpeed;

    readonly Maid StateMaid = new();

    public void TransitionedTo(AdvancedMovementState fromState)
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        doneDecelerating = false;
        controlLifted = false;
        Coroutine normalDecelerateRoutine = StartCoroutine(Decelerate(AccelerationCurve));
        StateMaid.GiveTask(() =>
        {
            if (normalDecelerateRoutine is not null)
            {
                StopCoroutine(normalDecelerateRoutine);
            }
        });

        Coroutine fastDecelerationRoutine = null;
        StateMaid.GiveEvent(
            Movement,
            "Jumped",
            (int _) =>
            {
                if (normalDecelerateRoutine is not null)
                {
                    StopCoroutine(normalDecelerateRoutine);
                }
                fastDecelerationRoutine = StartCoroutine(Decelerate(FastAccelerationCurve));
            }
        );
        StateMaid.GiveTask(() =>
        {
            if (fastDecelerationRoutine is not null)
            {
                StopCoroutine(fastDecelerationRoutine);
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
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    IEnumerator Decelerate(AnimationCurve curve)
    {
        float timeElapsed = 0;
        while (Movement.WalkSpeed > defaultWalkSpeed)
        {
            if (Movement.RawMovementVector == Vector3.zero)
            {
                controlLifted = true;
                Movement.WalkSpeed = defaultWalkSpeed;
                yield break;
            }
            Movement.WalkSpeed += curve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doneDecelerating = true;
        Movement.WalkSpeed = defaultWalkSpeed;
    }
}
