using System;
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

    private void OnEnable()
    {
        //Subscribe save/load actions
        DataPersistenceManager.Instance.onSaveTriggered += SaveData;
        DataPersistenceManager.Instance.onLoadTriggered += LoadData;
    }

    private void OnDisable()
    {
        //Unsubscribe save/load actions
        DataPersistenceManager.Instance.onSaveTriggered -= SaveData;
        DataPersistenceManager.Instance.onLoadTriggered -= LoadData;
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
            respawnPoint = newSpawn.Postion;
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
        DataPersistenceManager.Instance.SaveData(new SerializablePlayerRespawn(this.respawnPoint));
    }

    public void LoadData()
    {
        SerializablePlayerRespawn deserializedRespawn =
            DataPersistenceManager.Instance.LoadData(
                "respawnPoint",
                typeof(SerializablePlayerRespawn)
            ) as SerializablePlayerRespawn;
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
