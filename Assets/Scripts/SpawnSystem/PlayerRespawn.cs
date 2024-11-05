using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//attach to player
[Serializable]
public class PlayerRespawn : MonoBehaviour, IPersistableData
{
    [SerializeField]
    Checkpoint CurrentSpawn;

    [SerializeField]
    bool CanRevisitCheckpoints = true;

    CharacterMovement Movement => GetComponent<CharacterMovement>();

    HashSet<string> VisitedCheckpointIds = new();
    readonly Maid maid = new();

    private void OnEnable()
    {
        maid.GiveEvent(DataPersistenceManager.Instance, "onSaveTriggered", SaveData);
        maid.GiveEvent(DataPersistenceManager.Instance, "onLoadTriggered", LoadData);

        RespawnPlayer();
    }

    private void OnDisable()
    {
        maid.Cleanup();
    }

    bool SeenBefore(Checkpoint c)
    {
        return VisitedCheckpointIds.Contains(c.id);
    }

    bool CanSetSpawn(Checkpoint c)
    {
        return !SeenBefore(c) || CanRevisitCheckpoints;
    }

    private void OnTriggerEnter(Collider other)
    {
        //change respawn to current spawn
        if (!other.TryGetComponent(out Checkpoint newSpawn))
        {
            return;
        }

        if (!CanSetSpawn(newSpawn))
        {
            return;
        }

        CurrentSpawn = newSpawn;

        if (SeenBefore(newSpawn))
        {
            return;
        }

        VisitedCheckpointIds.Add(CurrentSpawn.id);
    }

    //spawn character in
    public void RespawnPlayer()
    {
        Movement.Warp(CurrentSpawn.RespawnAt.position, CurrentSpawn.RespawnAt.rotation);
    }

    public void SaveData()
    {
        DataPersistenceManager.SaveData(
            new SerializablePlayerRespawn(CurrentSpawn.id, VisitedCheckpointIds.ToListPooled())
        );
    }

    public void LoadData()
    {
        var deserializedRespawn =
            DataPersistenceManager.LoadData("checkpointID", typeof(SerializablePlayerRespawn))
            as SerializablePlayerRespawn;

        CurrentSpawn = Array.Find(
            FindObjectsByType<Checkpoint>(FindObjectsSortMode.InstanceID),
            c => c.id == deserializedRespawn.checkpointID
        );
        VisitedCheckpointIds = deserializedRespawn.visitedCheckpointIds.ToHashSet();
        RespawnPlayer();
    }
}

[Serializable]
public class SerializablePlayerRespawn
{
    public string checkpointID;
    public List<string> visitedCheckpointIds;

    public SerializablePlayerRespawn(string checkpointID, List<string> visitedIds)
    {
        this.checkpointID = checkpointID;
        this.visitedCheckpointIds = visitedIds;
    }
}
