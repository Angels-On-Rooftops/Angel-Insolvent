using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables
{
    //This class should eventually be updated to better match how the rest of the UI is set up
    public class OldDialogueInteractable : OldUIInteractable
    {
        //DialogueHandler dialogueHandler;

        void Awake()
        {
            GetInteractableOverlayComponent();
        }

        public override void Interact()
        {          
            //this.isActive = !this.isActive;

            if (!this.isActive)
            {
                //Mark Active when changing from inactive to active, but do not deactivate from this class
                this.isActive = true;

                StartCoroutine(CreateDialogueHandler());
            }
        }

        IEnumerator CreateDialogueHandler()
        {
            yield return new WaitForSeconds(.5f); //Wait to avoid the Interaction input interfering with the Move Forward input

            //Creates prefab in the center
            this.instantiatedUIPrefab = Instantiate(this.UIPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            DialogueHandler dialogueHandler = this.instantiatedUIPrefab.GetComponentInChildren<DialogueHandler>();
            dialogueHandler.EndDialogueNodeReached += DestroyDialogueInteractable;

            FreezeCharacterMovement();

            yield return null;
        }

        void DestroyDialogueInteractable()
        {
            Destroy(this.instantiatedUIPrefab);
            UnFreezeCharacterMovement();

            //this.dialogueHandler.EndDialogueNodeReached -= DestroyDialogueInteractable;
            //this.dialogueHandler = null;
            // ^commented out on the assumption that calling Destroy makes them unneeded

            this.isActive = false;
        }
    }
}
