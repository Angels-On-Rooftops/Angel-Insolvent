using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NpcInteractController : MonoBehaviour
{
    string postInteractText = "Dialogue";
    bool playerDetection = false;
    private Transform npc;
    public GameObject popup;
    public GameObject canvas;

    private void Start()
    {
        npc = transform.parent;
    }

    public void Interact()
    {
        Debug.Log("Test");
        if(playerDetection)
        {
            Debug.Log("NPC Interacted with player!");
            canvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = postInteractText;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            playerDetection = true;
            canvas.SetActive(playerDetection);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerDetection = false;
        canvas.SetActive(playerDetection);
    }
}
