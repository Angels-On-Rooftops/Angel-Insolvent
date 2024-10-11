using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Dialogue_System.DialogueSamples;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.Dialogue_System
{
    [Serializable]
    public class DialogueUIElements
    {
        public GameObject dialogueUIParent;
        public TMP_Text dialogueText;
        public TMP_Text characterNameText;
        [Space]
        public GameObject buttonPrefab;
        public GameObject buttonParent;
        public Vector2 firstButtonPosition;
        public Vector2 buttonDisplacement;
    }
    
    class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueUIElements defaultUIElements;
        [Space]
        [SerializeField]
        [Tooltip("The keybinds that allow the player to move forward in the dialogue (when not choosing a specific response)")]
        InputAction InteractAction;

        public static DialogueSystem Instance { get; private set; }

        public event Action EndOfDialogueReached;

        private bool conditionSatisfied = false;

        private DialogueUIElements currentUIElements = new DialogueUIElements();

        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            this.defaultUIElements.dialogueUIParent.SetActive(false);
        }

        private void FillCurrentUIElements(DialogueUIElements uniqueUIElements)
        {
            this.currentUIElements.dialogueUIParent
                = uniqueUIElements.dialogueUIParent != null ? uniqueUIElements.dialogueUIParent : this.defaultUIElements.dialogueUIParent;
            this.currentUIElements.dialogueText
                = uniqueUIElements.dialogueText != null ? uniqueUIElements.dialogueText : this.defaultUIElements.dialogueText;
            this.currentUIElements.characterNameText
                = uniqueUIElements.characterNameText != null ? uniqueUIElements.characterNameText : this.defaultUIElements.characterNameText;
            
            this.currentUIElements.buttonPrefab
                = uniqueUIElements.buttonPrefab != null ? uniqueUIElements.buttonPrefab : this.defaultUIElements.buttonPrefab;
            this.currentUIElements.buttonParent
                = uniqueUIElements.buttonParent != null ? uniqueUIElements.buttonParent : this.defaultUIElements.buttonParent;
            this.currentUIElements.firstButtonPosition
                = uniqueUIElements.firstButtonPosition != null ? uniqueUIElements.firstButtonPosition : this.defaultUIElements.firstButtonPosition;
            this.currentUIElements.buttonDisplacement
                = uniqueUIElements.buttonDisplacement != null ? uniqueUIElements.buttonDisplacement : this.defaultUIElements.buttonDisplacement;
        }

        public void PlayDialogue(DialogueFile file, DialogueUIElements uniqueUIElements, bool endDialogue = true)
        {
            FillCurrentUIElements(uniqueUIElements);

            this.currentUIElements.dialogueUIParent.SetActive(true);

            DialogueTree var = file.Dialogue;
            PlayDialogue(var);

            if (endDialogue)
            {
                this.currentUIElements.dialogueUIParent.SetActive(false);
                EndOfDialogueReached?.Invoke();
            }         
        }      

        private void PlayDialogue(DialogueTree tree)
        {
            foreach (DialogueNode d in tree.Nodes)
            {
                PlayDialogueNode(d);
            }
        }

        private void PlayDialogueNode(DialogueNode node)
        {
            Action toPlay = node switch
            {
                DialogueFrame frame => () => PlayFrame(frame),
                DialogueFireEvent dialogueEvent => () => FireOffEvent(dialogueEvent),
                DialogueBranch branch => () => DoBranch(branch),
                _ => throw new NotImplementedException(),
            };

            toPlay();
        }

        private void PlayFrame(DialogueFrame frame)
        {
            // edit the dialogue text
            this.currentUIElements.dialogueText.text = frame.BodyText;
            // edit the character name
            this.currentUIElements.characterNameText.text = frame.Character.Name;
            // play audio

            // wait until the continue condition is satisfied
            this.conditionSatisfied = false;

            if (frame.ContinueCondition is ContinueButtonHit)
            {
                InteractAction.performed += OnContinueButtonHit;
            }
            else if (frame.ContinueCondition is TimedContinue)
            {
                StartCoroutine(WaitThenEndPlayFrame(
                    ((TimedContinue)frame.ContinueCondition).Duration ) );
            }
            else if (frame.ContinueCondition is Choice)
            {

            }
            else
            {
                throw new NotImplementedException();
            }

            while (!this.conditionSatisfied) { }
            //may need to lock this variable since it shows up in multiple threads at the same time,
            //but I don't think it's needed in this particular case because it is not written to here
            //and if the code leaves the while loop a little later from needing to read the variable an extra time, it won't make a difference

            // stop audio
            // play dialogue tree from choice if that's something you have
        }

        void OnContinueButtonHit(CallbackContext c)
        {
            if (c.performed) //wait for full performance of action (ex. both a press and release of a button)
            {
                InteractAction.performed -= OnContinueButtonHit;
                StartCoroutine(WaitThenEndPlayFrame(.5f));
            }
        }

        IEnumerator WaitThenEndPlayFrame(float secondsToWait, DialogueFile fileChosen = null)
        {
            //After change pause, change to: yield return new WaitForSeconds(secondsToWait);
            yield return new WaitForSecondsRealtime(secondsToWait);

            // stop audio
            // play dialogue tree from choice if that's something you have
            if (fileChosen != null)
            {
                PlayDialogue(fileChosen, this.currentUIElements, false);
            }

            this.conditionSatisfied = true;
        }

        private void FireOffEvent(DialogueFireEvent fireEvent)
        {
            DialogueEvents.FireEvent(fireEvent.EventName);
        }

        private void DoBranch(DialogueBranch branch)
        {
            bool result = branch switch
            {
                HasItem hasItem => PlayerInventory.Instance.HasItem(hasItem.ItemId),
                ItemEquipped isEquipped => PlayerInventory.Instance.CurrentlyEquippedItem.itemID
                    == isEquipped.ItemId,
                _ => throw new NotImplementedException(),
            };

            PlayDialogue(result ? branch.OnTrue : branch.OnFalse);
        }
    }
}
