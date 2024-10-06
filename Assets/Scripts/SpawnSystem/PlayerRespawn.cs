using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;

//attach to player
[Serializable]
public class PlayerRespawn : MonoBehaviour, IPersistableData
{
    public Vector3 respawnPoint;

    [SerializeField]
    private Checkpoint checkPoint;

    [SerializeField]
    private bool CanRevisitCheckpoints = true;

    readonly Maid maid = new();

    private void OnEnable()
    {
        maid.GiveEvent(DataPersistenceManager.Instance, "onSaveTriggered", SaveData);
        maid.GiveEvent(DataPersistenceManager.Instance, "onLoadTriggered", LoadData);

        respawnPlayer();
    }

    private void OnDisable()
    {
        maid.Cleanup();
    }

    private void OnTriggerEnter(Collider other)
    {
        //change respawn to current spawn
        if (!other.gameObject.CompareTag("Respawn"))
        {
            return;
        }
        Checkpoint newSpawn = other.gameObject.GetComponent<Checkpoint>();
        if (!newSpawn.Activated || CanRevisitCheckpoints)
        {
            checkPoint = newSpawn;
            newSpawn.Activated = true;
        }
    }

    //spawn character in
    public void respawnPlayer()
    {
        transform.position = checkPoint.RespawnPosition;
    }

    public void SaveData()
    {
        DataPersistenceManager.SaveData(new SerializablePlayerRespawn(checkPoint.id));
    }

    // TODO what happens here if the player doesn't have a respawn point? do they just get teleported to 0,0,0 and fall through the map?
    public void LoadData()
    {
        var deserializedRespawn =
            DataPersistenceManager.LoadData("checkpointID", typeof(SerializablePlayerRespawn))
            as SerializablePlayerRespawn;

        checkPoint = Array.Find(FindObjectsByType<Checkpoint>(FindObjectsSortMode.InstanceID), c => c.id == deserializedRespawn.checkpointID);
        respawnPlayer();
    }
}

[Serializable]
public class SerializablePlayerRespawn
{
    public string checkpointID;

    public SerializablePlayerRespawn(string checkpointID)
    {
        this.checkpointID = checkpointID;
    }
}
