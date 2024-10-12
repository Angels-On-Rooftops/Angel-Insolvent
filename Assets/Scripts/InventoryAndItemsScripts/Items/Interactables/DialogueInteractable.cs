using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Dialogue_System;
using Assets.Scripts.Dialogue_System.DialogueSamples;
using Inventory;
using UnityEngine;
using UnityEngine.Assertions;

namespace Items.Interactables
{
    [RequireComponent(typeof(DialogueFile))]
    public class DialogueInteractable : UIInteractable
    {
        private DialogueFile DialogueFile => GetComponent<DialogueFile>();

        protected override void Awake()
        {
            Assert.IsNotNull(
                DialogueFile,
                $"You gotta put a dialogue file on this guy! {gameObject}"
            );

            base.Awake();
        }

        public override void Interact()
        {
            if (isActive)
            {
                return;
            }

            isActive = true;
            FreezeCharacterMovement();
            DialogueSystem.EndOfDialogueReached += OnEndOfDialogue;

            StartCoroutine(DialogueSystem.PlayDialogue(DialogueFile));
        }

        void OnEndOfDialogue()
        {
            DialogueSystem.EndOfDialogueReached -= OnEndOfDialogue;
            UnFreezeCharacterMovement();
            isActive = false;
        }

        //IEnumerator CreateDialogueHandler()
        //{
        //    /*yield return new WaitForSeconds(.5f); //Wait to avoid the Interaction input interfering with the Move Forward input

        //    //Creates prefab in the center
        //    this.instantiatedUIPrefab = Instantiate(this.UIPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        //    DialogueHandler dialogueHandler = this.instantiatedUIPrefab.GetComponentInChildren<DialogueHandler>();
        //    dialogueHandler.EndDialogueNodeReached += DestroyDialogueInteractable;

        //    FreezeCharacterMovement();*/

        //    yield return null;
        //}

        //void DestroyDialogueInteractable()
        //{
        //    //Destroy(this.instantiatedUIPrefab);
        //    UnFreezeCharacterMovement();

        //    //this.dialogueHandler.EndDialogueNodeReached -= DestroyDialogueInteractable;
        //    //this.dialogueHandler = null;
        //    // ^commented out on the assumption that calling Destroy makes them unneeded

        //    this.isActive = false;
        //}
    }
}
