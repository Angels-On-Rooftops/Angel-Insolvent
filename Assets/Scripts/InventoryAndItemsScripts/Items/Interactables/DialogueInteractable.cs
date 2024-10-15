using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DialogueSystem;
using Assets.Scripts.DialogueSystem.DialogueSamples;
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
    }
}
