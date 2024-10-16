using Assets.Scripts.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(DialogueFile))]
public class PopUpWhenTouch : MonoBehaviour
{
    private DialogueFile DialogueFile => GetComponent<DialogueFile>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided not player");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Collided");
            StartCoroutine(DialogueSystem.PlayDialogue(DialogueFile));
        }
    }
}
