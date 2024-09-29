using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deceleration : MonoBehaviour, IAdvancedMovementStateSpec
{
    [SerializeField]
    float Acceleration = -1;

    void Start()
    {
        defaultWalkSpeed = Movement.WalkSpeed;
        Acceleration = Mathf.Clamp(float.MinValue, 0, Acceleration);
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
        while (Movement.WalkSpeed > defaultWalkSpeed)
        {
            if (Movement.RawMovementVector == Vector3.zero)
            {
                doneDecelerating = true;
                yield break;
            }
            Movement.WalkSpeed += Acceleration * Time.deltaTime;
            yield return null;
        }
        doneDecelerating = true;
    }
}
