using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//attach to player
public class PlayerRespawn : MonoBehaviour
{

    public Vector3 respawnPoint;
    [SerializeField] Checkpoint checkPoint;


    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = checkPoint.Postion;
        respawnPlayer();
    }

   
    private void OnTriggerEnter(Collider other) {
        //change respawn to current spawn
        if (!other.gameObject.CompareTag("Respawn")) {
            return;
         }
        Checkpoint newSpawn = other.gameObject.GetComponent<Checkpoint>();
        if (!newSpawn.Activated) {
            respawnPoint = newSpawn.Postion;
            newSpawn.Activated = true;
         }
     }

    //spawn character in
    public void respawnPlayer() {
        this.transform.position = respawnPoint;
        }
    }
