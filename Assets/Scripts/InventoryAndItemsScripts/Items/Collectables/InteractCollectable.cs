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
        [SerializeField] private int numberOfTimesCanInteract = 1;
        [SerializeField] private GameObject player;
        
        private int numberOfTimesHasInteracted = 0;
        private bool willDestroyWhenDoneCollecting;

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
            this.willDestroyWhenDoneCollecting = this.destroyOnCollect;
            this.destroyOnCollect = false;
        }

        public void Interact()
        {
            this.numberOfTimesHasInteracted++;

            if (this.numberOfTimesHasInteracted >= this.numberOfTimesCanInteract)
            {
                this.destroyOnCollect = this.willDestroyWhenDoneCollecting;
            }

            if (this.numberOfTimesHasInteracted <= this.numberOfTimesCanInteract)
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