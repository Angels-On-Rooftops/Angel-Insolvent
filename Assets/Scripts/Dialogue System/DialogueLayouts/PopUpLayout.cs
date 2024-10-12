using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Dialogue_System.DialogueLayouts
{
    class PopUpLayout : MonoBehaviour, IDialogueLayout
    {
        [SerializeField]
        Canvas UILayout;

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
            throw new NotImplementedException();
        }

        public void SetCharacter(NarrativeCharacter character)
        {
            throw new NotImplementedException();
        }

        public Button[] SetChoiceButtons(Choice choice, Maid buttonCleaner)
        {
            throw new NotImplementedException();
        }
    }
}
