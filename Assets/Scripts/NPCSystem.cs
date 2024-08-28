using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCSystem : MonoBehaviour
{

    bool playerDetection = false;
    public GameObject canvas;


    // Update is called once per frame
    void Update()
    {
        if(playerDetection && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("NPC Interacted with player!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            playerDetection = true;
            canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerDetection = false;
        canvas.SetActive(false);
    }
}
