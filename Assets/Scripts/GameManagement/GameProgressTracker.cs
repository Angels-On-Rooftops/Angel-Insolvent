using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressTracker : MonoBehaviour, IPersistableData
{
    [Serializable]
    public enum GameProgress
    {
        None,
        VaultFinished,
        CityHallFinished,
        PowerPlantFinished,
        NewsStationFinished,
        YCorpFinished,
    }

    public GameProgress currentProgress = GameProgress.None;

    readonly Maid maid = new();

    private void OnEnable()
    {
        maid.GiveEvent(DataPersistenceManager.Instance, "onSaveTriggered", SaveData);
        maid.GiveEvent(DataPersistenceManager.Instance, "onLoadTriggered", LoadData);
    }

    private void OnDisable()
    {
        maid.Cleanup();
    }

    public void SaveData()
    {
        DataPersistenceManager.SaveData(this);
    }

    public void LoadData()
    {
        currentProgress = (GameProgress)DataPersistenceManager.LoadData("currentProgress", typeof(GameProgress));
    }
}
