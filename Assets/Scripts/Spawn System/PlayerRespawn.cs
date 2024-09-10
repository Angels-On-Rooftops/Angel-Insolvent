using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//attach to player
public class PlayerRespawn : MonoBehaviour
{
    GameObject player;
    SpawnPoint spawn;
    public Vector3 respawnPoint;
    //allows reuse of old checkpoints
    public bool allowBacktrack = false;



    // Start is called before the first frame update
    void Start()
    {
        player = this.gameObject;
        spawn = GameObject.FindGameObjectWithTag("Spawn").GetComponent<SpawnPoint>();
        //put player at leevel spawn to start 1st load of player
        
        respawnPoint = spawn.position;
        respawnPlayer();
    }

   
    private void OnTriggerEnter(Collider other) {
        //change respawn to current spawn
        if (other.gameObject.CompareTag("Respawn")) {
            Checkpoint newSpawn = other.gameObject.GetComponent<Checkpoint>();
            if (!newSpawn.activated) {
                respawnPoint = newSpawn.postion;
                newSpawn.activated = true;
                
                }
            else if (allowBacktrack) {
                respawnPoint = newSpawn.postion;
                }
            }
        }

    //spawn character in
    public void respawnPlayer() {
        player.transform.position = respawnPoint;
        }
    }
