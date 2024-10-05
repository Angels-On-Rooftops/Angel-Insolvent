using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
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
        maid.GiveEvent(DataPersistenceManager.Instance, "onSaveTriggered", LoadData);
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
            respawnPoint = newSpawn.RespawnPosition;
            newSpawn.Activated = true;
        }
    }

    //spawn character in
    public void respawnPlayer()
    {
        transform.position = respawnPoint;
    }

    public void SaveData()
    {
        DataPersistenceManager.SaveData(new SerializablePlayerRespawn(respawnPoint));
    }

    // TODO what happens here if the player doesn't have a respawn point? do they just get teleported to 0,0,0 and fall through the map?
    public void LoadData()
    {
        var deserializedRespawn =
            DataPersistenceManager.LoadData("respawnPoint", typeof(SerializablePlayerRespawn))
            as SerializablePlayerRespawn;

        respawnPoint = deserializedRespawn.respawnPoint;
        respawnPlayer();
    }
}

[Serializable]
public class SerializablePlayerRespawn
{
    public Vector3 respawnPoint;

    public SerializablePlayerRespawn(Vector3 respawnPoint)
    {
        this.respawnPoint = respawnPoint;
    }
}
