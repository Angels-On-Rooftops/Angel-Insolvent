using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePromptController : MonoBehaviour
{
    public void SaveAndExit()
    {
        DataPersistenceManager.Instance.SaveGame(new UnityEngine.InputSystem.InputAction.CallbackContext());
        GameStateManager.Instance.SetState(new MainMenuState());
        this.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }
}
