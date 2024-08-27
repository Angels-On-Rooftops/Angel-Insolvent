using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    public float radius = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // check all colliders within a given radius of the player after hitting "E".
            // NPCs should all have colliders that can be referenced
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider collider in colliderArray) {
                if (collider.TryGetComponent(out NPCInteractable npcInteractable))
                    npcInteractable.Interact(); // if the NPC is interactable, interact (crazy, right?)
            }
        }
    }
}
