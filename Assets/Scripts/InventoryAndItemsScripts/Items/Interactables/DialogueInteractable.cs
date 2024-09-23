using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables
{
    //This class should eventually be updated to better match how the rest of the UI is set up
    public class DialogueInteractable : UIInteractable
    {
        void Awake()
        {
            GetInteractableOverlayComponent();
        }

        public override void Interact()
        {
            this.isActive = !this.isActive;

            if (this.isActive)
            {
                //Creates prefab in the center
                this.instantiatedUIPrefab = Instantiate(this.UIPrefab, new Vector3(0, 0, 0), Quaternion.identity);

                FreezeCharacterMovement();
            }
            else
            {
                Destroy(this.instantiatedUIPrefab);

                UnFreezeCharacterMovement();
            }
        }
    }
}
