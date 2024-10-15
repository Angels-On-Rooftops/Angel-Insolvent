using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DialogueSystem.DialogueLayouts
{
    class PopUpLayout : MonoBehaviour, IDialogueLayout
    {
        [SerializeField]
        Canvas UILayout;

        [SerializeField]
        TMP_Text CharacterName;

        [SerializeField]
        TMP_Text BodyText;

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
            throw new NotImplementedException();
        }
    }
}
