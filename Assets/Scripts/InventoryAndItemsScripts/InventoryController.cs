using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The keybind that controls opening and closing the inventory.")]
    InputAction InventoryAction;

    [SerializeField]
    private GameObject inventory; // canvas game object

    private void OnEnable()
    {
        InventoryAction.performed += OpenInventory;
        InventoryAction.Enable();
    }

    private void OnDisable()
    {
        InventoryAction.performed -= OpenInventory;
        InventoryAction.Disable();
    }

    void OpenInventory(CallbackContext c)
    {
        // make sure something else isn't already pausing the game
        if (!PauseSystem.isPaused) {
            inventory.SetActive(!inventory.activeSelf);
            PauseSystem.PauseGame();
        }
    }
}
