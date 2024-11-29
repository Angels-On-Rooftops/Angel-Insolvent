using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
public class AdvancedMovement : MonoBehaviour
{
    InputAction ActionKeybind;

    public event Action ActionRequested;

    public AdvancedMovementState CurrentState = AdvancedMovementState.None;
    public event Action<AdvancedMovementState, AdvancedMovementState> StateChanged;

    Dictionary<AdvancedMovementState, IAdvancedMovementStateSpec> States =>
        new()
        {
            { AdvancedMovementState.None, GetComponent<DefaultMovement>() },
            { AdvancedMovementState.Rolling, GetComponent<Roll>() },
            { AdvancedMovementState.LongJumping, GetComponent<LongJump>() },
            { AdvancedMovementState.Plunging, GetComponent<Plunge>() },
            { AdvancedMovementState.Decelerating, GetComponent<Deceleration>() },
            { AdvancedMovementState.MoveStarting, GetComponent<MoveSpeedUp>() },
            { AdvancedMovementState.MoveStopping, GetComponent<MoveSpeedDown>() },
            { AdvancedMovementState.Gliding, GetComponent<Glide>() },
            { AdvancedMovementState.HighJumping, GetComponent<HighJump>() },
        };

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    CharacterController Controller => GetComponent<CharacterController>();
    readonly Maid StateMaid = new();

    // does not contain every default property, just ones that have been overwritten previously
    Dictionary<string, object> DefaultMovementProperties = new();

    HashSet<string> MovementPropertyExceptions = new() { "VerticalState" };

    private void Start()
    {
        foreach (IAdvancedMovementStateSpec movementState in States.Values)
        {
            Assert.IsFalse(
                DictionaryUtil.HasAnyKey(
                    movementState.MovementProperties,
                    movementState.HoldFromPreviousState
                ),
                $"Value both held and set from previous state in movement state {movementState.GetType().Name}!"
            );
        }
    }

    void OnEnable()
    {
        ActionKeybind = InputBindsHandler.Instance.FindBind("Movement Action");

        StateMaid.GiveEvent(
            ActionKeybind,
            "performed",
            (InputAction.CallbackContext c) => ActionRequested?.Invoke()
        );

        ActionKeybind.Enable();
        StateMaid.GiveTask(() => ActionKeybind.Disable());

        TransitionTo(CurrentState);
    }

    void OnDisable()
    {
        StateMaid.Cleanup();
    }

    void Update()
    {
        if (CanTransition(out AdvancedMovementState newState))
        {
            TransitionFrom(CurrentState, newState);
            TransitionTo(newState);
        }
    }

    // Returns true if a state change can occur according to the current state
    bool CanTransition(out AdvancedMovementState toTransitionTo)
    {
        IAdvancedMovementStateSpec stateSpec = States[CurrentState];

        foreach (var (newState, shouldTransitionTo) in stateSpec.Transitions)
        {
            if (shouldTransitionTo)
            {
                toTransitionTo = newState;
                return true;
            }
        }

        toTransitionTo = AdvancedMovementState.None;
        return false;
    }

    // Transitions back from a state
    void TransitionFrom(AdvancedMovementState oldState, AdvancedMovementState newState)
    {
        States[oldState].TransitioningFrom();

        Dictionary<string, object> DefaultMovementPropertiesMinusHoldProperties =
            DictionaryUtil.RemoveKeys(
                DefaultMovementProperties,
                States[newState].HoldFromPreviousState
            );

        SetMovementProperties(DefaultMovementPropertiesMinusHoldProperties);
    }

    // Transitions to a new state
    void TransitionTo(AdvancedMovementState state)
    {
        var oldState = CurrentState;
        CurrentState = state;

        SetMovementProperties(States[CurrentState].MovementProperties);

        States[state].TransitionedTo(oldState);
        StateChanged?.Invoke(oldState, CurrentState);
    }

    // Sets movement properties via a dictionary of the property names and values
    // returns a dictionary of all properties that were changed, with their old values
    void SetMovementProperties(Dictionary<string, object> newProperties)
    {
        foreach (var (name, newValue) in newProperties)
        {
            if (MovementPropertyExceptions.Contains(name))
            {
                continue;
            }

            var field = Movement.GetType().GetField(name);

            // check if name refers to a field or property
            // (because those are different in c# for some reason)
            if (field is not null)
            {
                DefaultMovementProperties.TryAdd(name, field.GetValue(Movement));
                field.SetValue(Movement, newValue);
            }
            else
            {
                var property = Movement.GetType().GetProperty(name);

                DefaultMovementProperties.TryAdd(name, property.GetValue(Movement));
                property.SetValue(Movement, newValue);
            }
        }
    }

    // enables the passed collider and disables the default movement collider
    // returns a method that will undo the change when ran
    public Action SetColliderHeight(float newHeight)
    {
        float oldHeight = Controller.height;
        Vector3 oldCenter = Controller.center;

        Controller.height = newHeight;
        Controller.center += Vector3.up * (newHeight - oldHeight) / 2;
        return () =>
        {
            Controller.height = oldHeight;
            Controller.center = oldCenter;
        };
    }
}
