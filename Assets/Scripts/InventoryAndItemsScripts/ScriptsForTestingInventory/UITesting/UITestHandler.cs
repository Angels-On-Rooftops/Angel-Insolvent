using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITestHandler : MonoBehaviour
{
    private EscMenuController escMenuController;

    public UITestHandler(InputAction menuAction, EscMenuController _escMenuController)
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
