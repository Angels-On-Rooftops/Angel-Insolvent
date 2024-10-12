using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Assets.Scripts.Dialogue_System.DialogueLayouts
{
    class TalkLayout : MonoBehaviour, IDialogueLayout
    {
        [SerializeField]
        Canvas UILayout;

        [SerializeField]
        Button[] ChoiceButtons;

        public void Disable()
        {
            UILayout.gameObject.SetActive(false);
        }

        public void Enable()
        {
            UILayout.gameObject.SetActive(true);
        }

        public void SetBodyText(string bodyText)
        {
            // TODO set body text
        }

        public void SetCharacter(NarrativeCharacter character)
        {
            // TODO set character name tag
        }

        public Button[] SetChoiceButtons(Choice choice, Maid buttonCleaner)
        {
            Assert.IsTrue(
                choice.Choices.Count < ChoiceButtons.Length,
                $"Can't support {choice.Choices.Count} choices!!!"
            );

            Button[] buttonsEnabled = new Button[choice.Choices.Count];

            // enable as many choice buttons as we need and set their text
            for (int i = 0; i < choice.Choices.Count; i++)
            {
                // TODO
                // (string choiceText, DialogueTree _) = choice.Choices[i]
                // ChoiceButtons[i].text = choiceText

                // button.SetActive(true);
                // buttonCleaner.GiveTask(() => button.SetActive(false);
            }

            return buttonsEnabled;
        }
    }
}
