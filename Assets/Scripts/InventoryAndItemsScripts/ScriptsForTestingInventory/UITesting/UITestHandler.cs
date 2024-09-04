using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITestHandler
{
    private EscMenuController escMenuController;
    private NpcInteractController interactController;

    private InputAction menuAction;
    private InputAction interactAction;

    public UITestHandler(InputAction _menuAction, InputAction _interactAction, EscMenuController _escMenuController, NpcInteractController _interactController)
    {
        menuAction = _menuAction;
        interactAction = _interactAction;

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

    public void Unbind()
    {
        menuAction.performed -= MenuAction_performed;
        interactAction.performed -= InteractAction_performed;
    }
}
