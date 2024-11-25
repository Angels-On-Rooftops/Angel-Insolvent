using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class InputBindsHandler
{
    private static InputBindsHandler instance = null;
    private static readonly object instanceLock = new object(); //thread-safe for co-routines

    private InputBinds inputBinds;
    private RebindingOperation rebindingOperation;

    InputBindsHandler()
    {
        inputBinds = new InputBinds();
    }

    public static InputBindsHandler Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new InputBindsHandler();
                }
                return instance;
            }
        }
    }

    public InputBinds GetInputBinds()
    {
        return inputBinds;
    }

    public InputAction FindBind(string bindName)
    {
        return inputBinds.FindAction(bindName);
    }

    public void OnRebindButtonClicked(InputAction action, int bindingIndex)
    {
        StartListeningForInput(action, bindingIndex);
    }

    private void StartListeningForInput(InputAction action, int bindingIndex)
    {
        if (rebindingOperation != null)
            rebindingOperation.Cancel();

        action.Disable();

        rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnMatchWaitForAnother(0.1f) // Prevent accidental double-input capture
            .OnComplete(operation => FinishListeningForInput(action, bindingIndex))
            .Start();
    }

    private void FinishListeningForInput(InputAction action, int bindingIndex)
    {
        rebindingOperation.Dispose();
        rebindingOperation = null;
        action.Enable();
    }

    public void SaveBind(string bindName)
    {
        var bind = FindBind(bindName);
        PlayerPrefs.SetString(bindName, bind.SaveBindingOverridesAsJson());
    }

    public void LoadBind(string bindName)
    {
        if(PlayerPrefs.HasKey(bindName))
        {
            string bindJson = PlayerPrefs.GetString(bindName);
            var bind = FindBind(bindName);
            bind.LoadBindingOverridesFromJson(bindJson);
        } else
        {
            Debug.Log($"No binding found for {bindName}");
        }
    }
}