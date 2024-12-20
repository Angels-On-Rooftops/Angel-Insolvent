﻿using System;
using System.Collections;
using Assets.Scripts.DialogueSystem.DialogueLayouts;
using Assets.Scripts.Libs;
using Inventory;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.DialogueSystem
{
    [RequireComponent(typeof(TalkLayout), typeof(PopUpLayout))]
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField]
        [Tooltip(
            "The keybinds that allow the player to move forward in dialogue (when not choosing a specific response)"
        )]
        InputAction InteractAction;

        public static event Action<string> EventFired;
        public static event Action EndOfDialogueReached;

        static readonly DialogueFlags flags = new();
        static DialogueSystem Instance { get; set; }

        private static readonly float inputWaitTime = 0.5f; //delay before allow input to prevent previous input from affecting the next input

        AudioSource DialogueAudio => GetComponent<AudioSource>();
        private static readonly string audioFolderName = "Audio/DialogueAudio"; //Must be in Assets/Resources

        static Maid dialogueMaid = new Maid();

        private void Awake()
        {
            InteractAction.Enable();

            Assert.IsNull(Instance, "More than 1 dialogue system detected!!!");
            Instance = this;
        }

        private void OnEnable()
        {
            flags.Enable();
        }

        private void OnDisable()
        {
            flags.Disable();
        }

        public static void SetDialogueVolume(float volume)
        {
            Instance.DialogueAudio.volume = volume;
        }

        public static IEnumerator PlayDialogue(DialogueFile file)
        {
            CharacterCamera camera = CameraUtil.GetPlayerCamera().GetComponent<CharacterCamera>();

            if (file.CameraPosition != null)
            {
                camera.enabled = false;
                dialogueMaid.GiveTask(() => camera.enabled = true);

                camera.transform.SetPositionAndRotation(
                    file.CameraPosition.position,
                    file.CameraPosition.rotation
                );
            }

            IDialogueLayout layout = file.LayoutType switch
            {
                DialogueLayoutType.Talk => Instance.GetComponent<TalkLayout>(),
                DialogueLayoutType.PopUp => Instance.GetComponent<PopUpLayout>(),
                _ => throw new NotImplementedException(),
            };

            layout.Enable();
            dialogueMaid.GiveTask(() => layout.Disable());

            yield return Instance.StartCoroutine(PlayDialogueTree(file.Dialogue, layout));

            EndOfDialogueReached?.Invoke();
            dialogueMaid.Cleanup();
        }

        public static IEnumerator PlayDialogueTree(DialogueTree tree, IDialogueLayout layout)
        {
            foreach (DialogueNode node in tree.Nodes)
            {
                yield return PlayDialogueNode(node, layout);
            }
        }

        private static IEnumerator PlayDialogueNode(DialogueNode node, IDialogueLayout layout)
        {
            Func<IEnumerator> toPlay = node switch
            {
                DialogueFrame frame => () => PlayFrame(frame, layout),
                DialogueFireEvent dialogueEvent => () => FireOffEvent(dialogueEvent),
                DialogueBranch branch => () => DoBranch(branch, layout),
                DialogueSetFlag setFlag => () => DoFlagNode(setFlag),
                _ => throw new NotImplementedException(),
            };

            yield return toPlay();
        }

        private static void PlayDialogueAudio(DialogueFrame frame)
        {
            if (frame.AudioFileName != null)
            {
                string audioFilePath = audioFolderName + "/" + frame.AudioFileName;
                AudioClip audioClip = Resources.Load<AudioClip>(audioFilePath);

                if (audioClip == null)
                {
                    Debug.LogError("Audio File at \"" + audioFilePath + "\" Not Found");
                }

                Instance.DialogueAudio.clip = audioClip;

                Instance.DialogueAudio.Play();
            }
        }

        private static IEnumerator PlayFrame(DialogueFrame frame, IDialogueLayout layout)
        {
            // edit text for this frame
            layout.SetCharacter(frame.Character);
            layout.SetBodyText(frame.BodyText);

            // play audio
            PlayDialogueAudio(frame);

            // wait until the continue condition is satisfied
            Maid continueMaid = new();
            int selectedChoice = -1;

            Func<IEnumerator> waitFor = frame.ContinueCondition switch
            {
                ContinueButtonHit button => () => WaitForContinueButton(continueMaid),
                TimedContinue timedContinue => () => WaitForTimer(timedContinue),
                Choice choiceContinue => () =>
                    DoChoiceSelection(
                        choiceContinue,
                        layout,
                        continueMaid,
                        (int n) =>
                        {
                            Debug.Log(n);
                            selectedChoice = n;
                        }
                    ),
                _ => throw new NotImplementedException(),
            };

            yield return Instance.StartCoroutine(waitFor());

            continueMaid.Cleanup();

            // stop audio
            Instance.DialogueAudio.Stop();

            // play dialogue tree from choice if that's something you have
            if (frame.ContinueCondition is Choice)
            {
                (string choiceText, DialogueTree tree) = (
                    frame.ContinueCondition as Choice
                ).Choices[selectedChoice];

                yield return PlayDialogueTree(tree, layout);
            }
        }

        static IEnumerator WaitForContinueButton(Maid continueMaid)
        {
            yield return new WaitForSeconds(inputWaitTime); // wait to prevent previous input from counting as a button press

            bool continueButtonHit = false;
            continueMaid.GiveEvent(
                Instance.InteractAction,
                "performed",
                (CallbackContext c) =>
                {
                    continueButtonHit = true;
                }
            );
            yield return new WaitUntil(() => continueButtonHit);
        }

        static IEnumerator WaitForTimer(TimedContinue timedContinue)
        {
            yield return new WaitForSeconds(timedContinue.Duration);
        }

        static IEnumerator DoChoiceSelection(
            Choice choice,
            IDialogueLayout layout,
            Maid continueMaid,
            Action<int> setSelectedChoice
        )
        {
            yield return new WaitForSeconds(inputWaitTime); // wait to prevent previous input from counting as a button press

            yield return WaitForChoiceSelection(
                layout.SetChoiceButtons(choice, continueMaid),
                continueMaid,
                setSelectedChoice
            );
        }

        static IEnumerator WaitForChoiceSelection(
            Button[] buttons,
            Maid continueMaid,
            Action<int> setSelectedChoice
        )
        {
            bool choiceSelected = false;

            for (int i = 0; i < buttons.Length; i++)
            {
                Debug.Log(i);

                int current = i; // we have to have this because we need to use the number in the event listener after the loop is complete
                buttons[i]
                    .onClick.AddListener(() =>
                    {
                        setSelectedChoice(current);
                        choiceSelected = true;
                    });

                continueMaid.GiveTask(buttons[current].onClick.RemoveAllListeners);
            }

            yield return new WaitUntil(() => choiceSelected);
        }

        private static IEnumerator FireOffEvent(DialogueFireEvent fireEvent)
        {
            EventFired?.Invoke(fireEvent.EventName);
            yield return null;
        }

        public static Action BindToEvent(string desiredEvent, Action method)
        {
            Action<string> callMethod = (string eventName) =>
            {
                if (eventName == desiredEvent)
                    method();
            };

            EventFired += callMethod;

            return () => EventFired -= callMethod;
        }

        private static IEnumerator DoBranch(DialogueBranch branch, IDialogueLayout layout)
        {
            bool result = branch switch
            {
                HasItem hasItem => PlayerInventory.Instance.HasItem(hasItem.ItemId),
                ItemEquipped isEquipped => PlayerInventory.Instance.CurrentlyEquippedItem.itemID
                    == isEquipped.ItemId,
                FlagCheck flagCheck => flags.FlagIsSet(flagCheck.Flag),
                _ => throw new NotImplementedException(),
            };

            yield return PlayDialogueTree(result ? branch.OnTrue : branch.OnFalse, layout);
        }

        private static IEnumerator DoFlagNode(DialogueSetFlag setFlag)
        {
            flags.SetFlag(setFlag.Flag, setFlag.Value);
            yield return null;
        }
    }
}
