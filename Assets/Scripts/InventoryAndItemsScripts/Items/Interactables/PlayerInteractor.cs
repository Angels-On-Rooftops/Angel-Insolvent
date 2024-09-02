using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Items.Interactables
{
    /// <summary>
    /// Handles player interaction with interactable objects
    /// (Singleton to make working with the UI easier)
    /// </summary>
    public class PlayerInteractor
    {
        private static PlayerInteractor instance = null;
        private static readonly object instanceLock = new object(); //thread-safe for co-routines

        /// <summary>
        /// null if none are in range; 
        /// otherwise, references the first interactable to come in range 
        /// (and returns back to null once out of range again)
        /// </summary>
        private IInteractable interactableCurrentlyInRange = null;

        private bool mayInteractWithCurrentInteractable = false;
        
        PlayerInteractor() { }

        public static PlayerInteractor Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new PlayerInteractor();
                    }
                    return instance;
                }
            }
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

            if (OnInInteractionRadius != null)
            {
                OnInInteractionRadius(mayInteract);
            }
        }

        public void Interact()
        {
            if (this.interactableCurrentlyInRange == null || !this.mayInteractWithCurrentInteractable)
            {
                Debug.LogError("Cannot Interact.");
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
                Debug.LogError("Interactable not currently in range.");
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

