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
    /// <summary>
    /// If you want to use the default for a field, leave it null
    /// </summary>
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

        private Stack<bool> conditionSatisfiedStack = new Stack<bool>(); //needs to be a stack to handle choices implementation
        //private readonly object conditionLock = new(); //whoops i thought co-routines were separate threads and they are not

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

        public void PlayDialogue(DialogueFile file, DialogueUIElements uniqueUIElements = null)
        {
            if (uniqueUIElements == null)
            {
                uniqueUIElements = new DialogueUIElements();
            }
            
            FillCurrentUIElements(uniqueUIElements);

            this.currentUIElements.dialogueUIParent.SetActive(true);

            //StartCoroutine(PlayDialogueInCoroutine(file)); 
            //PlayDialogueInCoroutine(file);
            StartCoroutine(PlayDialogueTree(file.Dialogue, true));
        }

        //needs to be a co-routine since it has to wait
        //(ends the dialogue, so do not use for for branching files in the middle of the dialogue)
        private void PlayDialogueInCoroutine(DialogueFile file)
        {
            StartCoroutine(PlayDialogueTree(file.Dialogue, true));

            //this.currentUIElements.dialogueUIParent.SetActive(false);
            //EndOfDialogueReached?.Invoke();

            //yield return null;
        }

        IEnumerator PlayDialogueTree(DialogueTree tree, bool endDialogue = false)
        {
            foreach (DialogueNode d in tree.Nodes)
            {
                this.conditionSatisfiedStack.Push(false);

                PlayDialogueNode(d);

                //Do not play next dialogue node until condition is met by a routine in PlayDialogueNode
                bool shouldBreakLoop = false;
                while (!shouldBreakLoop)
                {
                    bool conditionSatisfied = this.conditionSatisfiedStack.Pop();

                    if (conditionSatisfied)
                    {
                        shouldBreakLoop = true;
                    }
                    else
                    {
                        this.conditionSatisfiedStack.Push(false);
                    }

                    yield return null;
                }
            }

            if (endDialogue)
            {
                this.currentUIElements.dialogueUIParent.SetActive(false);
                EndOfDialogueReached?.Invoke();
            }
        }

        private void SetConditionSatisfiedToTrue()
        {
            this.conditionSatisfiedStack.Pop();
            this.conditionSatisfiedStack.Push(true);
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
            this.conditionSatisfiedStack.Push(false);          

            if (frame.ContinueCondition is ContinueButtonHit)
            {
                InteractAction.performed += OnContinueButtonHit;
                InteractAction.Enable();

                //while (!InteractAction.triggered) { }

                //StartCoroutine(WaitThenEndPlayFrame(.5f));

                //StartCoroutine(EnableButtonHitActionOnNewThread());
                //Check for button needs to be on a different thread than this one
                //because this thread will get blocked until the condition is met
            }
            else if (frame.ContinueCondition is TimedContinue)
            {
                StartCoroutine( WaitThenEndPlayFrame(
                    ((TimedContinue)frame.ContinueCondition).Duration ) );
            }
            else if (frame.ContinueCondition is Choice)
            {

            }
            else
            {
                throw new NotImplementedException();
            }

            /*bool shouldBreakLoop = false;
            while(!shouldBreakLoop)
            {
                bool conditionSatisfied = this.conditionSatisfiedStack.Pop();

                if (conditionSatisfied)
                {
                    shouldBreakLoop = true;
                }
                else
                {
                    this.conditionSatisfiedStack.Push(false);
                }

                yield return null;

                /*lock (this.conditionLock)
                {
                    bool conditionSatisfied = this.conditionSatisfiedStack.Pop();

                    if (conditionSatisfied)
                    {
                        break;
                    }
                    else
                    {
                        this.conditionSatisfiedStack.Push(false);
                    }
                }

                //wait briefly to make sure other threads get a chance to get the lock
                double time = 0;
                double timeStart = Time.realtimeSinceStartupAsDouble;
                while (time <= 0.05)
                {
                    time = Time.realtimeSinceStartup - timeStart;
                }*/
            //}
            
            //while (!this.conditionSatisfied) { }
            //may need to lock this variable since it shows up in multiple threads at the same time,
            //but I don't think it's needed in this particular case because it is not written to here
            //and if the code leaves the while loop a little later from needing to read the variable an extra time, it won't make a difference
            //(but I could see it causing a problem if the compiler/os makes a bad decision on what to cache, but I don't think it will)

            // stop audio
            // play dialogue tree from choice if that's something you have
        }

        /*IEnumerator EnableButtonHitActionOnNewThread()
        {
            InteractAction.performed += OnContinueButtonHit;
            yield return null;
        }*/

        void OnContinueButtonHit(CallbackContext c)
        {
            if (c.performed) //wait for full performance of action (ex. both a press and release of a button)
            {
                InteractAction.performed -= OnContinueButtonHit;
                InteractAction.Disable();

                StartCoroutine(WaitThenEndPlayFrame(.5f));
            }
        }

        IEnumerator WaitThenEndPlayFrame(float secondsToWait, DialogueFile fileChosen = null)
        {
            /*float time = 0;
            float timeStart = Time.realtimeSinceStartup; //remove after change pause
            while (time <= secondsToWait)
            {
                //After change pause, change to: time += Time.deltaTime;
                time = Time.realtimeSinceStartup - timeStart;
            }*/

            //After change pause, change to: yield return new WaitForSeconds(secondsToWait);
            yield return new WaitForSecondsRealtime(secondsToWait);

            // stop audio
            // play dialogue tree from choice if that's something you have
            if (fileChosen != null)
            {
                //Needs to NOT be a coroutine so that this.conditionSatisfied is not set to true until file is done being played
                PlayDialogueTree(fileChosen.Dialogue);
            }

            SetConditionSatisfiedToTrue();

            /*lock (this.conditionLock)
            {
                this.conditionSatisfiedStack.Push(true);
            }*/
        }

        private void FireOffEvent(DialogueFireEvent fireEvent)
        {
            DialogueEvents.FireEvent(fireEvent.EventName);
            SetConditionSatisfiedToTrue();
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

            SetConditionSatisfiedToTrue(); //<- needs to be before PlayDialogueTree so that after playing the new branch, the system knows to finish the current tree
            
            PlayDialogueTree(result ? branch.OnTrue : branch.OnFalse);
        }
    }
}
