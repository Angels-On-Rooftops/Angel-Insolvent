using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//one per scene
public class SpawnPoint : MonoBehaviour
{
    PlayerRespawn player;

    public bool hasCheckPoint = false; //has a checkpoint been reached
    public Vector3 position; //place to spawn character
    [SerializeField] int height = 1; //how high above repawn to position player
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawn>();
        position = new Vector3(this.transform.position.x, height,this.transform.position.z);

        // places player at level spawn at start of level 
          //2nd load onwards
        player.respawnPoint = position;
        player.respawnPlayer();
        
        
    }

    
}
