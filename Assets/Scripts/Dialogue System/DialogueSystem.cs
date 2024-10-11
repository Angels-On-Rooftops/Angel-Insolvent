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
using UnityEngine.UI;
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

        private List<GameObject> currentButtons = new List<GameObject>();

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

            //FillCurrentUIElements(uniqueUIElements);

            this.currentUIElements = this.defaultUIElements;

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

            if (frame.ContinueCondition is ContinueButtonHit)
            {
                InteractAction.performed += OnContinueButtonHit;
                InteractAction.Enable();
            }
            else if (frame.ContinueCondition is TimedContinue)
            {
                StartCoroutine( WaitThenEndPlayFrame(
                    ((TimedContinue)frame.ContinueCondition).Duration ) );
            }
            else if (frame.ContinueCondition is Choice)
            {
                CreateButtons(((Choice)frame.ContinueCondition).Choices);
            }
            else
            {
                throw new NotImplementedException();
            }            

            // stop audio
            // play dialogue tree from choice if that's something you have
        }

        private void CreateButtons(List<(string, DialogueTree)> choices)
        {
            Vector2 buttonPosition = this.currentUIElements.firstButtonPosition;
            foreach((string, DialogueTree) choice in choices)
            {
                CreateButton(buttonPosition, choice);
                buttonPosition += this.currentUIElements.buttonDisplacement;
            }

            if (this.currentButtons.Count > 0)
            {
                Button firstButton = this.currentButtons[0].GetComponentInChildren<Button>();
                firstButton.Select();
            }
        }

        private void CreateButton(Vector2 buttonPosition, (string choiceText, DialogueTree tree) choice)
        {
            GameObject newButton = Instantiate(this.currentUIElements.buttonPrefab, 
                buttonPosition, Quaternion.identity, this.currentUIElements.buttonParent.transform);

            Button buttonComponent = newButton.GetComponentInChildren<Button>();
            buttonComponent.onClick.AddListener(delegate { OnChoiceButtonPress(choice); });

            newButton.GetComponentInChildren<TMP_Text>().text = choice.choiceText;

            this.currentButtons.Add(newButton);

            SetLastButtonNavigation();
        }

        private void SetLastButtonNavigation()
        {
            int index = this.currentButtons.Count - 1;

            if (index <= 0)
            {
                return;
            }

            Button lastButton = this.currentButtons[index].GetComponentInChildren<Button>();
            Button prevButton = this.currentButtons[index - 1].GetComponentInChildren<Button>();

            Navigation lastButtonNav = new Navigation();
            lastButtonNav.mode = Navigation.Mode.Explicit;

            Navigation prevButtonNav;
            if (index == 1)
            {
                prevButtonNav = new Navigation();
                prevButtonNav.mode = Navigation.Mode.Explicit;
            }
            else
            {
                prevButtonNav = prevButton.navigation;
            }

            lastButtonNav.selectOnLeft = prevButton;
            prevButtonNav.selectOnRight = lastButton;

            lastButtonNav.selectOnUp = prevButton;
            prevButtonNav.selectOnDown = lastButton;

            lastButton.navigation = lastButtonNav;
            prevButton.navigation = prevButtonNav;
        }

        void OnChoiceButtonPress((string choiceText, DialogueTree tree) choice)
        {
            StartCoroutine(WaitThenEndPlayFrame(.5f, choice.tree));

            //Clear buttons
            for (int i = this.currentButtons.Count - 1; i >= 0; i--)
            {
                Destroy(this.currentButtons[i]);
            }
            this.currentButtons.Clear();
        }

        void OnContinueButtonHit(CallbackContext c)
        {
            if (c.performed) //wait for full performance of action (ex. both a press and release of a button)
            {
                InteractAction.performed -= OnContinueButtonHit;
                InteractAction.Disable();

                StartCoroutine(WaitThenEndPlayFrame(.5f));
            }
        }

        IEnumerator WaitThenEndPlayFrame(float secondsToWait, DialogueTree fileChosenTree = null)
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

            SetConditionSatisfiedToTrue(); //<- needs to be before PlayDialogueTree so that after playing the new branch, the system knows to finish the current tree

            // stop audio
            // play dialogue tree from choice if that's something you have
            if (fileChosenTree != null)
            {
                yield return StartCoroutine(PlayDialogueTree(fileChosenTree)); //wait for this to finish
            }
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

            StartCoroutine(WaitThenEndPlayFrame(0, result ? branch.OnTrue : branch.OnFalse));

            //SetConditionSatisfiedToTrue(); //<- needs to be before PlayDialogueTree so that after playing the new branch, the system knows to finish the current tree
            
            //PlayDialogueTree(result ? branch.OnTrue : branch.OnFalse);
        }
    }
}
