using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
public class AdvancedMovement : MonoBehaviour
{
    [SerializeField]
    InputAction ActionKeybind;

    [SerializeField]
    float Length = 1;

    [SerializeField]
    float RollingSpeed = 12;

    [SerializeField]
    Collider StandingCollider;

    [SerializeField]
    Collider RollingCollider;

    CharacterMovement movement;
    bool IsRolling;

    public AdvancedMovementState CurrentState = AdvancedMovementState.None;

    void Awake()
    {
        movement = GetComponent<CharacterMovement>();
    }

    void OnEnable()
    {
        Keybind.performed += DoRollInput;
        Keybind.Enable();

        movement.Landed += StopRoll;
    }

    void OnDisable()
    {
        Keybind.performed -= DoRollInput;
        Keybind.Disable();

        movement.Landed -= StopRoll;
    }

    bool CanRoll()
    {
        return !IsRolling;
    }

    void DoRollInput(InputAction.CallbackContext c)
    {
        if (!CanRoll())
        {
            return;
        }

        StartCoroutine(DoRoll());
    }

    public IEnumerator DoRoll()
    {
        StartRoll();
        yield return new WaitForSeconds(Length);
        StopRoll();
    }

    void StartRoll()
    {
        if (IsRolling)
        {
            return;
        }

        IsRolling = true;
        movement.WalkSpeed = 30;
        movement.JumpHeight = 3;
        RollingCollider.gameObject.SetActive(true);
        StandingCollider.gameObject.SetActive(false);
    }

    void StopRoll()
    {
        if (!IsRolling)
        {
            return;
        }

        movement.WalkSpeed = 16;
        movement.JumpHeight = 6;
        IsRolling = false;
        RollingCollider.gameObject.SetActive(false);
        StandingCollider.gameObject.SetActive(true);
    }
}
