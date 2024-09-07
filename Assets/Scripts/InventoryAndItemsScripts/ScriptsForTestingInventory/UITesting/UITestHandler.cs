using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITestHandler
{
    private EscMenuController escMenuController;

    public UITestHandler(InputAction menuAction, InputAction interactAction, EscMenuController _escMenuController, NpcInteractController _interactController)
    {
        menuAction.performed += MenuAction_performed;
        menuAction.Enable();

        this.escMenuController = _escMenuController;
    }
    private void MenuAction_performed(InputAction.CallbackContext obj)
    {
        escMenuController.MenuToggle();
    }
}
