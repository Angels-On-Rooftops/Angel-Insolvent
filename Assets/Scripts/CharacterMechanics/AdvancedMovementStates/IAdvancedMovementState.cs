using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdvancedMovementState
{
    None,
    Rolling,
    Diving,
    LongJumping,
}


public interface IAdvancedMovementStateSpec
{
    public Dictionary<AdvancedMovementState, bool> Transitions { get; }
    public Dictionary<string, object> MovementProperties { get; }

    public void TransitionedTo();

    public void TransitioningFrom();
}
