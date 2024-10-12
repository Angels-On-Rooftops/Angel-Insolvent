using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

namespace Items.Interactables
{
    //This class should eventually be updated to better match how the rest of the UI is set up
    public class UIInteractable : MonoBehaviour, IInteractable
    {
        protected bool isActive = false;
        protected InteractableOverlayHelper interactableOverlay;

        public bool DestroyAfterInteracting
        {
            get { return false; }
        }

        protected virtual void Awake()
        {
            GetInteractableOverlayComponent();
        }

        protected void GetInteractableOverlayComponent()
        {
            this.interactableOverlay = this.GetComponentInChildren<InteractableOverlayHelper>();
            if (this.interactableOverlay is null)
            {
                Debug.LogError("Child of Interactable should have InteractableOverlayHelper");
            }
        }

        public void DisableInteractableCanvas()
        {
            this.interactableOverlay?.DisableCanvas();
        }

        public void EnableInteractableCanvas()
        {
            this.interactableOverlay?.EnableCanvas();
        }

        public virtual void Interact()
        {
            this.isActive = !this.isActive;

            if (this.isActive)
            {
                FreezeCharacterMovement();
            }
            else
            {
                UnFreezeCharacterMovement();
            }
        }

        public bool MeetsCriteriaToInteract(Collider playerCollider)
        {
            return true;
        }

        protected void FreezeCharacterMovement()
        {
            //Pause game (should later replace with game's pause system)
            Time.timeScale = 0;
            //set character controller not active
            //this.player.GetComponent<CharacterController>().enabled = false;
        }

        protected void UnFreezeCharacterMovement()
        {
            //Pause game (should later replace with game's pause system)
            Time.timeScale = 1;
            //set character controller active
            //this.player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
