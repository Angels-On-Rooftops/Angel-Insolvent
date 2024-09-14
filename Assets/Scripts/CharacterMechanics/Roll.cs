using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterMovement))]
public class Roll : MonoBehaviour
{
    [SerializeField]
    InputAction Keybind;

    [SerializeField]
    float Length = 1;

    [SerializeField]
    float Boost = 12;

    [SerializeField]
    Collider StandingCollider;

    [SerializeField]
    Collider RollingCollider;

    CharacterMovement movement;
    bool IsRolling;

    void Awake()
    {
        movement = GetComponent<CharacterMovement>();
    }

    void OnEnable()
    {
        Keybind.performed += DoRollInput;
        Keybind.Enable();

        movement.JumpRequested += StopRoll;
    }

    void OnDisable()
    {
        Keybind.performed -= DoRollInput;
        Keybind.Disable();

        movement.JumpRequested -= StopRoll;
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
        movement.WalkSpeed += Boost;
        RollingCollider.gameObject.SetActive(true);
        StandingCollider.gameObject.SetActive(false);
    }

    void StopRoll()
    {
        if (!IsRolling)
        {
            return;
        }

        movement.WalkSpeed -= Boost;
        IsRolling = false;
        RollingCollider.gameObject.SetActive(false);
        StandingCollider.gameObject.SetActive(true);
    }
}
