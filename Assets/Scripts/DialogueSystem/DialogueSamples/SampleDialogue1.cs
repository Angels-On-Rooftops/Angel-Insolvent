using System.Collections.Generic;

namespace Assets.Scripts.DialogueSystem.DialogueSamples
{
    public class SampleDialogue1 : DialogueFile
    {
        public override DialogueLayoutType LayoutType => DialogueLayoutType.Talk;

        public override DialogueTree Dialogue =>
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.OverheadSpeaker,
                        "All security personnel to the tuning fork vault.",
                        new ContinueButtonHit()
                    ),
                    new DialogueFrame(
                        Characters.OverheadSpeaker,
                        "Code Yellow, break-in from suspected Angel, take precautions.",
                        new ContinueButtonHit(),
                        "30 second groov"
                    ),
                    new DialogueFrame(
                        Characters.Opus,
                        "You have fifteen security guards headed your way. "
                            + "I’ll find you a path out – you need to hurry.",
                        new Choice(
                            new List<(string, DialogueTree)>()
                            {
                                ("Okay!", AcceptMission),
                                ("No.", DenyMission),
                            }
                        ),
                        "intro level"
                    ),
                    new DialogueFrame(
                        Characters.OverheadSpeaker,
                        "Check flag (should get Deny):",
                        new ContinueButtonHit()
                    ),
                    new FlagCheck( "sampleCheckSeen",
                        AcceptMission,
                        DenyMission
                    ),
                    new DialogueSetFlag("sampleCheckSeen", true),
                    new DialogueFrame(
                        Characters.OverheadSpeaker,
                        "Check flag (should get Accept):",
                        new ContinueButtonHit()
                    ),
                    new FlagCheck( "sampleCheckSeen",
                        AcceptMission,
                        DenyMission
                    ),
                    new DialogueFrame(
                        Characters.OverheadSpeaker,
                        "Should fire event:",
                        new ContinueButtonHit()
                    ),
                    new DialogueFireEvent("RancidVibes"),
                    new DialogueFrame(
                        Characters.OverheadSpeaker,
                        "THE POLICE ARE ON THEIR WAY!!!!!",
                        new TimedContinue(2f)
                    ),
                }
            );

        DialogueTree AcceptMission =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(Characters.Opus, "Cool!", new ContinueButtonHit()),
                }
            );

        DialogueTree DenyMission =
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Opus,
                        "Don't play games with me.",
                        new ContinueButtonHit()
                    ),
                }
            );
    }
}
