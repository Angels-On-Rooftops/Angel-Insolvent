using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputBindsHandler
{
    private static InputBindsHandler instance = null;
    private static readonly object instanceLock = new object(); //thread-safe for co-routines

    private InputBinds inputBinds;

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
}
