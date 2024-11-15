using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//attach to player
[Serializable]
public class PlayerRespawn : MonoBehaviour, IPersistableData
{
    [SerializeField]
    CheckpointTrigger CurrentSpawn;

    [SerializeField]
    bool CanRevisitCheckpoints = true;

    CharacterMovement Movement => GetComponent<CharacterMovement>();

    HashSet<string> VisitedCheckpointIds = new();
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

    bool SeenBefore(CheckpointTrigger c)
    {
        return VisitedCheckpointIds.Contains(c.id);
    }

    bool CanSetSpawn(CheckpointTrigger c)
    {
        return !SeenBefore(c) || CanRevisitCheckpoints;
    }

    private void OnTriggerEnter(Collider other)
    {
        //change respawn to current spawn
        if (!other.TryGetComponent(out CheckpointTrigger newSpawn))
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
        if (CurrentSpawn != null)
        {
            Movement.Warp(CurrentSpawn.RespawnAt.position, CurrentSpawn.RespawnAt.rotation);
        }
    }

    public void SaveData()
    {
        var id = CurrentSpawn?.id ?? "";
        DataPersistenceManager.SaveData(
            new SerializablePlayerRespawn(id, VisitedCheckpointIds.ToListPooled())
        );
    }

    public void LoadData()
    {
        var deserializedRespawn =
            DataPersistenceManager.LoadData("checkpointID", typeof(SerializablePlayerRespawn))
            as SerializablePlayerRespawn;

        CurrentSpawn = Array.Find(
            FindObjectsByType<CheckpointTrigger>(FindObjectsSortMode.InstanceID),
            c => c.id == deserializedRespawn.checkpointID
        );
        VisitedCheckpointIds = deserializedRespawn.visitedCheckpointIds.ToHashSet();
        //RespawnPlayer();
    }
}

[Serializable]
public class SerializablePlayerRespawn
{
    [SerializeField]
    public string checkpointID;

    [SerializeField]
    public List<string> visitedCheckpointIds;

    public SerializablePlayerRespawn(string checkpointID, List<string> visitedIds)
    {
        this.checkpointID = checkpointID;
        this.visitedCheckpointIds = visitedIds;
    }
}
