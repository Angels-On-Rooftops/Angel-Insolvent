using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdvancedMovementState
{
    None,
    Rolling,
    Dive,
    LongJump,
}


public abstract class IAdvancedMovementState : MonoBehaviour
{
    float MovementSpeed;
    float JumpHeight;
    Collider Collider;

    Dictionary<Action, AdvancedMovementState> TransitionEvents;
}
