using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class NpcInteractController : MonoBehaviour
{
    string postInteractText = "Dialogue";
    bool playerDetection = false;
    private Transform npc;
    // public GameObject popup;  // this isn't being used? I don't think we need it
    public GameObject canvas;

    [SerializeField]
    [Tooltip("The keybinds that control lateral character movement. To restrict character movement to a single axis, unbind the directions you don't want accessible.")]
    InputAction Interact;

    private void Start()
    {
        npc = transform.parent;
    }

    private void OnEnable()
    {
        Interact.performed += DoInteraction;
        Interact.Enable();
    }

    private void OnDisable()
    {
        Interact.performed -= DoInteraction;
        Interact.Disable();
    }

    public void DoInteraction(CallbackContext c)
    {
        if(playerDetection)
        {
            Debug.Log("NPC Interacted with player!");
            // canvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = postInteractText;
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
