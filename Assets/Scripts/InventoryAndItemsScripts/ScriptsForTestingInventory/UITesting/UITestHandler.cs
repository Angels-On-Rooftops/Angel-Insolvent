using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITestHandler
{
    private EscMenuController escMenuController;
    private NpcInteractController interactController;

    public UITestHandler(InputAction menuAction, InputAction interactAction, EscMenuController _escMenuController, NpcInteractController _interactController)
    {
        menuAction.performed += MenuAction_performed;
        menuAction.Enable();

        interactAction.performed += InteractAction_performed;
        interactAction.Enable();

        this.escMenuController = _escMenuController;
        this.interactController = _interactController;
    }
    private void MenuAction_performed(InputAction.CallbackContext obj)
    {
        escMenuController.MenuToggle();
    }

    private void InteractAction_performed(InputAction.CallbackContext obj)
    {
        interactController.Interact();
    }
}
