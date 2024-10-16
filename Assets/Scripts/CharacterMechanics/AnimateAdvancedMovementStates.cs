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

    readonly Maid maid = new();



    void OnEnable()
    {
        maid.GiveEvent(BasicMovementController, "StartedWalking", () => SetWalk(true));
        maid.GiveEvent(BasicMovementController, "StoppedWalking", () => SetWalk(false));

        maid.GiveEvent(BasicMovementController, "Jumped", (int _) => SetTrigger("Jump"));
        maid.GiveEvent(BasicMovementController, "Falling", () => SetTrigger("Fall"));

        maid.GiveEvent(BasicMovementController, "Landed", () => ResetTrigger("Fall"));
        maid.GiveEvent(BasicMovementController, "Landed", () => ResetTrigger("Jump"));
        maid.GiveEvent(BasicMovementController, "Landed", () => SetTrigger("Land"));

        Dictionary<AdvancedMovementState, Action> StateChanges = new()
        {
            { AdvancedMovementState.Rolling, () => {
                Animator.ResetTrigger("RollStop");
                SetTrigger("RollStart");
            } },
            { AdvancedMovementState.LongJumping, () => SetTrigger("LongJump") },
            { AdvancedMovementState.HighJumping, () => SetTrigger("HighJump") },
            { AdvancedMovementState.Gliding, () => SetTrigger("Glide") },
            { AdvancedMovementState.Plunging, () => SetTrigger("Plunge") },
        };

        maid.GiveEvent(
            AdvancedMovementController, 
            "StateChanged", 
            (AdvancedMovementState last, AdvancedMovementState next) =>
            {
                if (StateChanges.ContainsKey(next))
                {
                    StateChanges[next]();
                    return;
                }
                
            }
        );

        maid.GiveEvent(
            AdvancedMovementController, 
            "StateChanged", 
            (AdvancedMovementState last, AdvancedMovementState next) =>
            {
                Debug.Log($"{last} {next}");
                if (last == AdvancedMovementState.Rolling)
                {
                    SetTrigger("RollStop");
                }
            }
        );
    }

    void OnDisable()
    {
        maid.Cleanup();
    }

    void SetTrigger(string state)
    {
        Animator.SetTrigger(state);
    }

    void ResetTrigger(string state)
    {
        Animator.ResetTrigger(state);
    }

    void SetWalk(bool state)
    {
        Animator.SetBool("IsMoving", state);
    }

    private void Update()
    {
        Animator.SetFloat("RunSpeed", (BasicMovementController.WalkSpeed * BasicMovementController.MovementDirection).magnitude/16);
    }
}
