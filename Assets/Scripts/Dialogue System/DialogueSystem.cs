using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Dialogue_System.DialogueSamples;
using Inventory;

namespace Assets.Scripts.Dialogue_System
{
    class DialogueSystem
    {
        public void PlayDialogue(DialogueFile file)
        {
            PlayDialogue(file.Dialogue);
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
            // play audio
            // edit the dialogue text
            // edit the character name

            // wait until the continue condition is satisfied

            // stop audio
            // play dialogue tree from choice if that's something you have
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
