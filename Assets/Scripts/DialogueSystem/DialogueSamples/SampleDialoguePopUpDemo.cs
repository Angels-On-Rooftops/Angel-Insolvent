using System.Collections.Generic;

namespace Assets.Scripts.DialogueSystem.DialogueSamples
{
    public class SampleDialoguePopUpDemo : DialogueFile
    {
        public override DialogueLayoutType LayoutType => DialogueLayoutType.PopUp;

        public override DialogueTree Dialogue =>
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Coda,
                        "Hey, can they see what I'm saying now?",
                        new TimedContinue(4.5f),
                        "PopUpDemo_1"
                    ),
                    new DialogueFrame(
                        Characters.Opus,
                        "Yes, they should be receiving these messages now.",
                        new TimedContinue(4f),
                        "PopUpDemo_2"
                    ),
                    new DialogueFrame(
                        Characters.Opus,
                        "Addtionally, these messages will play in sequence without them needing to input anything on their end,",
                        new TimedContinue(7f),
                        "PopUpDemo_3"
                    ),
                    new DialogueFrame(
                        Characters.Opus,
                        "and they can see which of us is speaking by looking at the name on the display.",
                        new TimedContinue(5f),
                        "PopUpDemo_4"
                    ),
                    new DialogueFrame(
                        Characters.Coda,
                        "Awesome!",
                        new TimedContinue(2f),
                        "PopUpDemo_5"
                    ),
                }
            );
    }
}
