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
    private InputAction loadAction; //only needed for testing

    public event Action onSaveTriggered;
    public event Action onLoadTriggered;

    private FileDataHandler fileDataHandler;
    private string fileName = "AngelInsolventSaveFile.txt";

    DataPersistenceManager()
    {
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        saveAction = new InputAction(binding: "<Keyboard>/f5"); //should make this serializable somewhere for multiple platforms' inputs
        saveAction.performed += SaveGame;
        saveAction.Enable();

        loadAction = new InputAction(binding: "<Keyboard>/f6");
        loadAction.performed += LoadGame;
        loadAction.Enable();
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

    //Save methods
    public void SaveGame(CallbackContext context)
    {
        fileDataHandler.OpenFileSave();
        onSaveTriggered?.Invoke();
        fileDataHandler.CloseFileSave();
    }

    public void SaveData(object data)
    {
        if(data != null)
        {
            fileDataHandler.WriteObjectToJson(data);
        }
    }

    //Load methods
    public void LoadGame(CallbackContext context)
    {
        fileDataHandler.OpenFileLoad();
        onLoadTriggered?.Invoke();
        fileDataHandler.CloseFileLoad();
    }

    public object LoadData(Type type)
    {
        return fileDataHandler.ReadObjectFromJson(type);
    }
}
