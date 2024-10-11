using Assets.Scripts.Dialogue_System;
using Assets.Scripts.Dialogue_System.DialogueSamples;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables
{
    //This class should eventually be updated to better match how the rest of the UI is set up
    public class DialogueInteractable : UIInteractable
    {
        [SerializeField] private GameObject dialogueFileObject;
        [SerializeField] private DialogueUIElements dialogueUIElements;

        private DialogueFile dialogueFile;

        protected override void Awake()
        {
            this.dialogueFile = this.dialogueFileObject.GetComponent<DialogueFile>();
            if (this.dialogueFile is null)
            {
                Debug.LogError("DialogueInteractable should have a dialogueFileObject with a DialogueFile script atteched to it.");
            }

            base.Awake();          
        }

        public override void Interact()
        {
            if (this.isActive == false)
            {
                DialogueSystem.Instance.PlayDialogue(this.dialogueFile, this.dialogueUIElements);
                Debug.Log("Playing Dialogue");

                FreezeCharacterMovement();
                this.isActive = true;
                DialogueSystem.Instance.EndOfDialogueReached += OnEndOfDialogue;
            }         
        }

        void OnEndOfDialogue()
        {
            UnFreezeCharacterMovement();
            this.isActive = false;
            DialogueSystem.Instance.EndOfDialogueReached -= OnEndOfDialogue;
        }

        IEnumerator CreateDialogueHandler()
        {
            /*yield return new WaitForSeconds(.5f); //Wait to avoid the Interaction input interfering with the Move Forward input

            //Creates prefab in the center
            this.instantiatedUIPrefab = Instantiate(this.UIPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            DialogueHandler dialogueHandler = this.instantiatedUIPrefab.GetComponentInChildren<DialogueHandler>();
            dialogueHandler.EndDialogueNodeReached += DestroyDialogueInteractable;

            FreezeCharacterMovement();*/

            yield return null;
        }

        void DestroyDialogueInteractable()
        {
            //Destroy(this.instantiatedUIPrefab);
            UnFreezeCharacterMovement();

            //this.dialogueHandler.EndDialogueNodeReached -= DestroyDialogueInteractable;
            //this.dialogueHandler = null;
            // ^commented out on the assumption that calling Destroy makes them unneeded

            this.isActive = false;
        }
    }
}
