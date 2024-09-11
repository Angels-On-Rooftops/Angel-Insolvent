using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager
{
    private static DataPersistenceManager instance = null;
    private static readonly object instanceLock = new object(); //thread-safe for co-routines

    public event Action onSaveTriggered;

    DataPersistenceManager() { }

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

    public void SaveGame()
    {
        onSaveTriggered?.Invoke();
    }
}
