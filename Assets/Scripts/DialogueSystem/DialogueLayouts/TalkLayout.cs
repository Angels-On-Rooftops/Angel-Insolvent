using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Assets.Scripts.DialogueSystem.DialogueLayouts
{
    class TalkLayout : MonoBehaviour, IDialogueLayout
    {
        [SerializeField]
        Canvas UILayout;

        [SerializeField]
        TMP_Text CharacterName;

        [SerializeField]
        TMP_Text BodyText;

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
            BodyText.text = bodyText;
        }

        public void SetCharacter(NarrativeCharacter character)
        {
            CharacterName.text = character.Name;
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
                (string choiceText, DialogueTree _) = choice.Choices[i];
                ChoiceButtons[i].GetComponentInChildren<TMP_Text>().text = choiceText;

                Button button = ChoiceButtons[i];

                button.gameObject.SetActive(true);
                buttonCleaner.GiveTask(() => button.gameObject.SetActive(false));

                buttonsEnabled[i] = ChoiceButtons[i];
            }

            buttonsEnabled[0].Select();

            return buttonsEnabled;
        }
    }
}
