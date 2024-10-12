using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Items.Interactables;

namespace Items.Collectables
{
    public class InteractCollectable : Collectable, IInteractable
    {
        [SerializeField] private int timesCanInteract = 1;
        [SerializeField] private GameObject player;
        
        private int numberTimesInteracted = 0;
        private bool destroyWhenDone;
        private InteractableOverlayHelper interactableOverlay;

        /// <summary>
        /// Use this property AFTER calling Interact()
        /// </summary>
        public bool DestroyAfterInteracting
        {
            get
            {
                return this.destroyOnCollect;
            }
        }

        void Awake()
        {
            this.destroyWhenDone = this.destroyOnCollect;
            this.destroyOnCollect = false;

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

        public void Interact()
        {
            this.numberTimesInteracted++;

            if (this.numberTimesInteracted >= this.timesCanInteract)
            {
                this.destroyOnCollect = this.destroyWhenDone;
            }

            if (this.numberTimesInteracted <= this.timesCanInteract)
            {
                StartCoroutine(CollectionRoutine(this.player));
            }
        }

        public bool MeetsCriteriaToInteract(Collider playerCollider)
        {
            InventoryChecker checker = this.gameObject.GetComponent<InventoryChecker>();

            if (checker != null)
            {
                return checker.InventoryStateMeetsCriteria();
            }
            
            return true;
        }
    }
}
