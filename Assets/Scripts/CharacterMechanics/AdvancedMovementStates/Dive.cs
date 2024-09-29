using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dive : MonoBehaviour, IAdvancedMovementStateSpec
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
        };
    public Dictionary<string, object> MovementProperties =>
        new()
        {
            { "GravityMultiplier", FallAcceleration },
            { "DownwardTerminalVelocity", TerminalVelocity },
            { "JumpHeight", 0 }
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed" };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();

    bool landed,
        hitWall;

    Maid StateMaid = new();

    bool doneDecelerating = false;

    public void TransitionedTo()
    {
        hitWall = false;
        StateMaid.GiveEvent(Movement, "RanIntoWall", () => hitWall = true);

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
            if (Movement.RawMovementVector == Vector3.zero)
            {
                doneDecelerating = true;
                Movement.WalkSpeed = MinAdjustSpeed;
                yield break;
            }
            Movement.WalkSpeed += HorizAccelerationCurve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doneDecelerating = true;
        Movement.WalkSpeed = MinAdjustSpeed;
    }
}
