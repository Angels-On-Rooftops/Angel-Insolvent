using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables.Dialogue
{
    //This class should eventually be updated to better match how the rest of the UI is set up
    public class DialogueInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject UIDialoguePrefab;

        private bool isActive = false;
        private GameObject instantiatedUIPrefab;
        private InteractableOverlayHelper interactableOverlay;

        public bool DestroyAfterInteracting { get { return false; } }

        void Awake()
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

        public void Interact()
        {
            this.isActive = !this.isActive;

            if (this.isActive)
            {
                //Creates prefab in the center
                this.instantiatedUIPrefab = Instantiate(this.UIDialoguePrefab, new Vector3(0, 0, 0), Quaternion.identity);

                //Pause game (should later replace with game's pause system)
                Time.timeScale = 0;
            }
            else
            {
                Destroy(this.instantiatedUIPrefab);

                //UnPause game (should later replace with game's pause system)
                Time.timeScale = 1;
            }
        }

        public bool MeetsCriteriaToInteract(Collider playerCollider)
        {
            return true;
        }
    }
}
