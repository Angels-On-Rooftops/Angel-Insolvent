using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class DataPersistenceManager
{
    private static DataPersistenceManager instance = null;
    private static readonly object instanceLock = new object(); //thread-safe for co-routines

    private GameManagementActions dataPersistenceActions;
    private InputAction saveAction;
    private InputAction loadAction; //only needed for testing

    public event Action onSaveTriggered;
    public event Action onLoadTriggered;

    private FileDataHandler fileDataHandler;
    private string fileName = "AngelInsolventSaveFile.txt";

    DataPersistenceManager()
    {
        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

        dataPersistenceActions = new GameManagementActions();

        saveAction = dataPersistenceActions.Actions.SaveGame;
        saveAction.performed += SaveGame;
        saveAction.Enable();

        loadAction = dataPersistenceActions.Actions.LoadGame;
        loadAction.performed += LoadGame;
        loadAction.Enable();
    }

    ~DataPersistenceManager()
    {
        saveAction.performed -= SaveGame;
        loadAction.performed -= LoadGame;
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
        try
        {
            fileDataHandler.OpenFileSave();
            onSaveTriggered?.Invoke();
            fileDataHandler.CloseFileSave();

            Debug.Log("Game saved");
        }
        catch (Exception e)
        {
            Debug.Log("Game not saved: " + e.Message);
            fileDataHandler.CloseFileSave();
        }
    }

    public static void SaveData(object data)
    {
        if (data == null)
        {
            return;
        }

        Instance.fileDataHandler.WriteObjectToJson(data);
    }

    //Load methods
    public void LoadGame(CallbackContext context)
    {
        try
        {
            fileDataHandler.OpenFileLoad();
            onLoadTriggered?.Invoke();
            fileDataHandler.CloseFileLoad();

            Debug.Log("Game loaded");
        }
        catch (FileNotFoundException)
        {
            Debug.Log("Save file not found, starting new game");
        } catch(Exception e)
        {
            Debug.Log("Game not loaded: " + e.Message);
            fileDataHandler.CloseFileLoad();
        }
    }

    public static object LoadData(string jsonTag, Type returnType)
    {
        return Instance.fileDataHandler.ReadObjectFromJson(jsonTag, returnType);
    }
}
