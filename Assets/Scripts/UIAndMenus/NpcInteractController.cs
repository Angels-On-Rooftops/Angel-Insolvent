using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class NpcInteractController : MonoBehaviour
{
    bool playerDetection = false;
    private Transform npc;
    public float radius;
    
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    [Tooltip("The keybinds that control character interaction with NPCs and objects in the environment.")]
    InputAction Interact;

    private void Start()
    {
        npc = transform.parent;
        npc.GetComponentInChildren<SphereCollider>().radius = radius;
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
            Debug.Log("Interacted with player!");
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
