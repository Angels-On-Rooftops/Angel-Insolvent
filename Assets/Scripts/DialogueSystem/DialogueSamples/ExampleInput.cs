using System.Collections.Generic;

namespace Assets.Scripts.DialogueSystem.DialogueSamples
{
    public class ExampleInput : DialogueFile
    {
        public override DialogueLayoutType LayoutType => DialogueLayoutType.Talk;

        public override DialogueTree Dialogue =>
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(Characters.Coda, "Wait", new TimedContinue(4), "file.wav"),
                    new DialogueFrame(
                        Characters.Opus,
                        "This has no button audio, which should be fine. "
                            + "Hit the interact button to continue the dialogue.",
                        new ContinueButtonHit()
                    ),
                    new DialogueFrame(
                        Characters.Clef,
                        "Here's a dialogue choice",
                        new Choice(
                            new List<(string, DialogueTree)>
                            {
                                ("first choice", DialogueChoice1),
                                ("another choice", DialogueChoice2),
                                ("a third choice", DialogueChoice3),
                            }
                        )
                    ),
                    new DialogueFireEvent("Spoke"),
                    new DialogueSetFlag("AcceptedMission", true),
                    new HasItem("TuningFork", DialogueHasTuningFork, DialogueMissingTuningFork),
                    new ItemEquipped(
                        "TuningFork",
                        DialogueHasTuningFork,
                        DialogueMissingTuningFork
                    ),
                    new FlagCheck("TestFlag", TestFlagTrue, TestFlagFalse),
                }
            );
    }
}
