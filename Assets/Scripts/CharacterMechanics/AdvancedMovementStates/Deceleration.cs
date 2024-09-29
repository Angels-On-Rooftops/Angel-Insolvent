using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deceleration : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    AnimationCurve AccelerationCurve;

    void Start()
    {
        defaultWalkSpeed = Movement.WalkSpeed;
    }

    public Dictionary<AdvancedMovementState, bool> Transitions =>
        new()
        {
            { AdvancedMovementState.None, doneDecelerating },
            { AdvancedMovementState.Rolling, pushedActionButton && Movement.IsOnGround() },
            { AdvancedMovementState.Diving, pushedActionButton && !Movement.IsOnGround() },
        };

    public List<string> HoldFromPreviousState => new() { "WalkSpeed" };

    public Dictionary<string, object> MovementProperties => new() { };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    AdvancedMovement AdvancedMovement => GetComponent<AdvancedMovement>();
    bool pushedActionButton = false;
    bool doneDecelerating = false;
    float defaultWalkSpeed;

    readonly Maid StateMaid = new();

    public void TransitionedTo()
    {
        pushedActionButton = false;
        StateMaid.GiveEvent(AdvancedMovement, "ActionRequested", () => pushedActionButton = true);

        doneDecelerating = false;
        Coroutine decelerateRoutine = StartCoroutine(Decelerate());
        StateMaid.GiveTask(() =>
        {
            if (decelerateRoutine is not null)
            {
                StopCoroutine(decelerateRoutine);
            }
        });
    }

    public void TransitioningFrom()
    {
        StateMaid.Cleanup();
    }

    IEnumerator Decelerate()
    {
        float timeElapsed = 0;
        while (Movement.WalkSpeed > defaultWalkSpeed)
        {
            if (Movement.RawMovementVector == Vector3.zero)
            {
                doneDecelerating = true;
                yield break;
            }
            Movement.WalkSpeed += AccelerationCurve.Evaluate(timeElapsed) * Time.deltaTime;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doneDecelerating = true;
    }
}
