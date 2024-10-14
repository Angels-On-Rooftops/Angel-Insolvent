using System.Collections.Generic;

namespace Assets.Scripts.DialogueSystem.DialogueSamples
{
    public class EventTest : DialogueFile
    {
        public override DialogueLayoutType LayoutType => DialogueLayoutType.Talk;

        public override DialogueTree Dialogue =>
            new(
                new List<DialogueNode>()
                {
                    new DialogueFireEvent("RancidVibes") 
                }
            );
    }
}
