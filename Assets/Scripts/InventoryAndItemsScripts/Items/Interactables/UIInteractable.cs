using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables
{
    //This class should eventually be updated to better match how the rest of the UI is set up
    public class UIInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] protected GameObject UIPrefab;

        protected bool isActive = false;
        protected GameObject instantiatedUIPrefab;
        protected InteractableOverlayHelper interactableOverlay;

        public bool DestroyAfterInteracting { get { return false; } }

        void Awake()
        {
            GetInteractableOverlayComponent();

            //Creates prefab in the center
            this.instantiatedUIPrefab = Instantiate(this.UIPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            this.instantiatedUIPrefab.SetActive(false);
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
                this.instantiatedUIPrefab?.SetActive(true);

                FreezeCharacterMovement();
            }
            else
            {
                this.instantiatedUIPrefab?.SetActive(false);

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
        }

        protected void UnFreezeCharacterMovement()
        {
            //Pause game (should later replace with game's pause system)
            Time.timeScale = 1;
            //set character controller active
        }
    }
}
