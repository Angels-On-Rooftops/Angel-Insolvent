using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class DataPersistenceManager
{
    private static DataPersistenceManager instance = null;
    private static readonly object instanceLock = new object(); //thread-safe for co-routines

    private InputAction saveAction;
    public event Action onSaveTriggered;

    private FileDataHandler fileDataHandler;
    private string fileName = "AngelInsolventSaveFile.txt";

    DataPersistenceManager()
    {
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        saveAction = new InputAction(binding: "<Keyboard>/f5"); //should make this serializable somewhere for multiple platform
        saveAction.performed += SaveGame;
        saveAction.Enable();
    }

    public static DataPersistenceManager Instance
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new DataPersistenceManager();
                }
                return instance;
            }
        }
    }

    public void SaveGame(CallbackContext context)
    {
        fileDataHandler.OpenFile();
        onSaveTriggered?.Invoke();
        fileDataHandler.CloseFile();
    }

    public void SaveData(object data)
    {
        if(data != null)
        {
            fileDataHandler.WriteObjectToJson(data);
        }
    }
}
