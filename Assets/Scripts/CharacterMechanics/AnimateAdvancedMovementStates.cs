using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimateAdvancedMovementStates : MonoBehaviour
{
    [SerializeField]
    CharacterMovement BasicMovementController;

    [SerializeField]
    AdvancedMovement AdvancedMovementController;

    Animator Animator => GetComponent<Animator>();

    readonly Dictionary<string, AdvancedMovementState> advancedStateBindings =
        new()
        {
            { "Rolling", AdvancedMovementState.Rolling },
            { "HighJumping", AdvancedMovementState.HighJumping },
            { "LongJumping", AdvancedMovementState.LongJumping },
            { "Plunging", AdvancedMovementState.Plunging },
            { "Gliding", AdvancedMovementState.Gliding },
        };

    readonly Dictionary<string, VerticalMovementState> verticalStateBindings =
        new()
        {
            { "Jumping", VerticalMovementState.Jumping },
            { "Falling", VerticalMovementState.Falling },
            { "Grounded", VerticalMovementState.Grounded },
        };

    private void Update()
    {
        bool inAdvancedState = false;

        foreach ((string animatorBool, AdvancedMovementState state) in advancedStateBindings)
        {
            bool shouldSet = AdvancedMovementController.CurrentState == state;
            Animator.SetBool(animatorBool, shouldSet);
            inAdvancedState |= shouldSet;
        }

        Animator.SetBool("InAdvancedState", inAdvancedState);

        foreach ((string animatorBool, VerticalMovementState state) in verticalStateBindings)
        {
            Animator.SetBool(animatorBool, BasicMovementController.VerticalState == state);
        }

        Animator.SetBool("IsMoving", BasicMovementController.MovementDirection.magnitude != 0);

        float moveSpeed = (
            BasicMovementController.WalkSpeed * BasicMovementController.MovementDirection
        ).magnitude;
        float moveSpeedToAnimationScale = 1 / 16f;

        if (moveSpeed != 0)
        {
            Animator.SetFloat("RunSpeed", moveSpeed * moveSpeedToAnimationScale);
        }
    }
}
