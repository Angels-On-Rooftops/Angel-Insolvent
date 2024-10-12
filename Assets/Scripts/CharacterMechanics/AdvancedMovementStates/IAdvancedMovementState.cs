using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdvancedMovementState
{
    None,
    Rolling,
    Plunging,
    LongJumping,
    Decelerating,
    MoveStarting,
    MoveStopping,
    Gliding,
    HighJumping
}

public interface IAdvancedMovementStateSpec
{
    public Dictionary<AdvancedMovementState, bool> Transitions { get; }
    public Dictionary<string, object> MovementProperties { get; }

    public List<string> HoldFromPreviousState { get; }

    public void TransitionedTo(AdvancedMovementState oldState);

    public void TransitioningFrom();
}
