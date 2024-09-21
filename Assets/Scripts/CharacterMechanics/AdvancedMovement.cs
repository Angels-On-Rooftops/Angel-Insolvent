using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
public class AdvancedMovement : MonoBehaviour
{
    [SerializeField]
    InputAction ActionKeybind;

    public event Action ActionRequested;

    public AdvancedMovementState CurrentState = AdvancedMovementState.None;
    public event Action<AdvancedMovementState, AdvancedMovementState> StateChanged;

    Dictionary<AdvancedMovementState, IAdvancedMovementStateSpec> States
    {
        get => new()
            {
                { AdvancedMovementState.None, GetComponent<DefaultMovement>() },
                { AdvancedMovementState.Rolling, GetComponent<Roll>() },
                { AdvancedMovementState.LongJumping, GetComponent<LongJump>() },
                { AdvancedMovementState.Diving, GetComponent<Dive>() },
            };
    }

    CharacterMovement Movement => GetComponent<CharacterMovement>();
    readonly Maid StateMaid = new();

    Dictionary<string, object> PreCurrentStateMovementProperties = new();

    void OnEnable()
    {
        StateMaid.GiveEvent<Action>(
            () => ActionRequested?.Invoke(),
            func => ActionKeybind.performed += (_ => func()),
            func => ActionKeybind.performed -= (_ => func())
        );

        ActionKeybind.Enable();
        StateMaid.GiveTask(() => ActionKeybind.Disable());

        TransitionTo(CurrentState);
        StateMaid.GiveTask(() => TransitionFrom(CurrentState));
    }

    void OnDisable()
    {
        StateMaid.Cleanup();
    }

    void Update()
    {
        if (CanTransition(out AdvancedMovementState newState))
        {
            TransitionFrom(CurrentState);
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
    void TransitionFrom(AdvancedMovementState state)
    {
        States[state].TransitioningFrom();
        SetMovementProperties(PreCurrentStateMovementProperties);
    }

    // Transitions to a new state
    void TransitionTo(AdvancedMovementState state)
    {
        var oldState = CurrentState;
        CurrentState = state;

        PreCurrentStateMovementProperties = SetMovementProperties(States[CurrentState].MovementProperties);

        States[state].TransitionedTo();
        StateChanged?.Invoke(oldState, CurrentState);
    }

    // Sets movement properties via a dictionary of the property names and values
    // returns a dictionary of all properties that were changed, with their old values
    Dictionary<string, object> SetMovementProperties(Dictionary<string, object> newProperties)
    {
        Dictionary<string, object> oldProps = new();

        foreach (var (name, newValue) in newProperties)
        {
            var field = Movement.GetType().GetField(name);

            // check if name refers to a field or property
            // (because those are different in c# for some reason)
            if (field is not null) {
                oldProps.Add(name, field.GetValue(Movement));
                field.SetValue(Movement, newValue);
            } 
            else
            {
                var property = Movement.GetType().GetProperty(name);

                oldProps.Add(name, property.GetValue(Movement));
                property.SetValue(Movement, newValue);
            }
        }

        return oldProps;
    }
}
