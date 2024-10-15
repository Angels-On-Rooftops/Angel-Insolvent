using System.Collections.Generic;

namespace Assets.Scripts.DialogueSystem.DialogueSamples
{
    public class SampleDialogueDemoNodes : DialogueFile
    {
        public override DialogueLayoutType LayoutType => DialogueLayoutType.Talk;

        public override DialogueTree Dialogue =>
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Resonara,
                        "Hello. I'm Resonara, as you can see from my name text—"
                            + "and before you ask, it does appear that I have briefly"
                                + " become aware of the fourth wall for the purposes of this demonstration.",
                        new ContinueButtonHit(),
                        "NodesDemo_1"
                    ),
                    new DialogueFrame(
                        Characters.Resonara,
                        "Now, for which of these do you have an inquiry?",
                        new Choice(
                            new List<(string, DialogueTree)>()
                            {
                                ("Saved Flags", SavedFlags),
                                ("Inventory Check", InventoryCheck),
                                ("Fire Event", FireEvent)
                            }
                        ),
                        "NodesDemo_2"
                    ),
                    new DialogueFrame(
                        Characters.Resonara,
                        "It was truly a delight speaking with you.",
                        new ContinueButtonHit(),
                        "NodesDemo_9"
                    )
                }
            );

        DialogueTree FireEvent =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFireEvent("ChangeMaterial"),
                    new DialogueFrame(
                        Characters.Resonara,
                        "As you can observe, an event was fired to alter my appearance.",
                        new ContinueButtonHit(),
                        "NodesDemo_3"
                    ),                 
                }
            );

        DialogueTree InventoryCheck =
            new(
                new List<DialogueNode>()
                {
                    new HasItem( "carrot",
                        HasCarrot,
                        DoesNotHaveCarrot
                    ),

                }
            );

        static DialogueTree HasCarrot =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Resonara,
                        "I see that you have, in fact, collected the carrot. Congratulations.",
                        new ContinueButtonHit(),
                        "NodesDemo_4"
                    ),
                }
            );

        static DialogueTree DoesNotHaveCarrot =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Resonara,
                        "I see that you have NOT collected the carrot yet.",
                        new ContinueButtonHit(),
                        "NodesDemo_5"
                    ),
                }
            );

        DialogueTree SavedFlags =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Resonara,
                        "I'm taking note that you have selected this path to observe Saved Flags.",
                        new ContinueButtonHit(),
                        "NodesDemo_6"
                    ),
                    new FlagCheck( "savedFlagAnEvenNumberOfTimes",
                        SavedFlagsEven,
                        SavedFlagsOdd
                    ),
                    
                }
            );

        static DialogueTree SavedFlagsEven =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Resonara,
                        "According to my records, you have selected this path an EVEN number of times.",
                        new ContinueButtonHit(),
                        "NodesDemo_7"
                    ),
                    new DialogueSetFlag("savedFlagAnEvenNumberOfTimes", false),
                }
            );

        static DialogueTree SavedFlagsOdd =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Resonara,
                        "According to my records, you have selected this path an ODD number of times.",
                        new ContinueButtonHit(),
                        "NodesDemo_8"
                    ),
                    new DialogueSetFlag("savedFlagAnEvenNumberOfTimes", true),
                }
            );
    }
}
