using System.Collections.Generic;

namespace Assets.Scripts.DialogueSystem.DialogueSamples
{
    public class SampleDialoguePopUpDemo : DialogueFile
    {
        public override DialogueLayoutType LayoutType => DialogueLayoutType.Talk;
        //public override DialogueLayoutType LayoutType => DialogueLayoutType.PopUp;

        public override DialogueTree Dialogue =>
            new(
                new List<DialogueNode>()
                {
                    new DialogueFrame(
                        Characters.Coda,
                        "Hey, can they see what I'm saying now?",
                        new TimedContinue(4f)
                    ),
                    new DialogueFrame(
                        Characters.Opus,
                        "Yes, they should be receiving these messages now.",
                        new TimedContinue(4f)
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
