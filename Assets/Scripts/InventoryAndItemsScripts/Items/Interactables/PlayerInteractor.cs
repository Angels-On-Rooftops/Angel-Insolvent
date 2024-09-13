using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Items.Interactables
{
    /// <summary>
    /// Handles player interaction with interactable objects
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The keybinds that control character interaction with NPCs and objects in the environment.")]
        InputAction InteractAction;

        /// <summary>
        /// null if none are in range; 
        /// otherwise, references the first interactable to come in range 
        /// (and returns back to null once out of range again)
        /// </summary>
        private IInteractable interactableCurrentlyInRange = null;

        private bool mayInteractWithCurrentInteractable = false;

        private void OnEnable()
        {
            InteractAction.performed += Interact;
            InteractAction.Enable();
        }

        private void OnDisable()
        {
            InteractAction.performed -= Interact;
            InteractAction.Disable();
        }

        /// <summary>
        /// Triggered when a PlayerInteractor that was previously not colliding
        /// with an InteractionCollider begins colliding with one
        /// (takes in a bool that is true when the criteria to interact has been met)
        /// </summary>
        public event Action<bool> OnInInteractionRadius;

        public event Action OnLeaveInteractionRadius;

        public event Action<bool> CanInteractUpdate;

        public void InInteractionRadius(bool mayInteract, IInteractable interactable)
        {
            if (this.interactableCurrentlyInRange != null)
            {
                return;
            }

            this.interactableCurrentlyInRange = interactable;
            this.mayInteractWithCurrentInteractable = mayInteract;

            interactable.EnableInteractableCanvas();

            if (OnInInteractionRadius != null)
            {
                OnInInteractionRadius(mayInteract);
            }
        }

        void Interact(CallbackContext c)
        {
            if (this.interactableCurrentlyInRange == null || !this.mayInteractWithCurrentInteractable)
            {
                Debug.Log("Cannot Interact.");
                return;
            }

            this.interactableCurrentlyInRange.Interact();

            if (this.interactableCurrentlyInRange.DestroyAfterInteracting)
            {
                this.interactableCurrentlyInRange = null;
                this.mayInteractWithCurrentInteractable = false;
            }
        }

        public void LeaveInteractionRadius()
        {
            if (this.interactableCurrentlyInRange == null)
            {
                return;
            }
            this.interactableCurrentlyInRange.DisableInteractableCanvas();

            this.interactableCurrentlyInRange = null;
            this.mayInteractWithCurrentInteractable = false;

            if (OnLeaveInteractionRadius != null)
            {
                OnLeaveInteractionRadius();
            }
        }

        //May remove this if it causes performance issues
        public void CheckWhetherCanStillInteract(Collider playerCollider)
        {
            if (this.interactableCurrentlyInRange == null)
            {
                Debug.Log("Interactable not currently in range.");
                return;
            }

            bool previous = this.mayInteractWithCurrentInteractable;
            this.mayInteractWithCurrentInteractable = this.interactableCurrentlyInRange.MeetsCriteriaToInteract(playerCollider);

            if (previous != this.mayInteractWithCurrentInteractable)
            {
                if (CanInteractUpdate != null)
                {
                    CanInteractUpdate(this.mayInteractWithCurrentInteractable);
                }
            }
        }
    }
}

