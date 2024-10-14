using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SavePromptController : MonoBehaviour
{
    [SerializeField] private EscMenuController escMenuController;

    public void SaveAndExit()
    {
        DataPersistenceManager.Instance.SaveGame(new UnityEngine.InputSystem.InputAction.CallbackContext());
        GameStateManager.Instance.SetState(new MainMenuState());
        this.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        escMenuController.CloseSavePrompt();
    }
}
