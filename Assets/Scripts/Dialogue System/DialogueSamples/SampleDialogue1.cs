using System.Collections.Generic;

namespace Assets.Scripts.Dialogue_System.DialogueSamples
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
                        new ContinueButtonHit()
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
                        )
                    ),
                    new DialogueFrame(
                        Characters.OverheadSpeaker,
                        "THE POLICE ARE ON THEIR WAY!!!!!",
                        new TimedContinue(5)
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
