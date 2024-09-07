using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables
{
    /// <summary>
    /// Script for objects defining the collider area for an interactable object
    /// (should be placed on the child of the interactable object that defines
    /// the area around the interactable object where the player may interact with the object)
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class InteractionCollider : MonoBehaviour
    {
        private IInteractable interactable;

        private void Awake()
        {
            this.interactable = this.GetComponentInParent<IInteractable>();
            if (interactable == null)
            {
                Debug.LogError("Parent of InteractionCollider must have IInteractable component.");
                return;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<PlayerInteractor>().InInteractionRadius
                    (this.interactable.MeetsCriteriaToInteract(other), this.interactable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<PlayerInteractor>().LeaveInteractionRadius();
            }
        }

        //May remove this if it causes performance issues
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<PlayerInteractor>().CheckWhetherCanStillInteract(other);
            }
        }
    }
}
