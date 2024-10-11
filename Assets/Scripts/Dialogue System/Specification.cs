using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace Assets.Scripts.Dialogue_System
{
    public record DialogueTree(List<DialogueNode> Nodes) { }

    public record DialogueNode();

    public record DialogueFrame(
        NarrativeCharacter Character,
        string BodyText,
        DialogueContinue ContinueCondition,
        string AudioFileName = null
    ) : DialogueNode();

    public record DialogueFireEvent(string EventName) : DialogueNode();

    public record DialogueSetFlag(string Flag, bool Value) : DialogueNode();

    public record DialogueBranch(DialogueTree OnTrue, DialogueTree OnFalse) : DialogueNode();

    public record HasItem(string ItemId, DialogueTree YesItem, DialogueTree NoItem)
        : DialogueBranch(YesItem, NoItem);

    public record ItemEquipped(string ItemId, DialogueTree Equipped, DialogueTree NotEquipped)
        : DialogueBranch(Equipped, NotEquipped);

    public record FlagValue(string Flag, DialogueTree OnTrue, DialogueTree OnFalse)
        : DialogueBranch(OnTrue, OnFalse);

    public record CheckDialogueCondition(
        string ItemId,
        DialogueTree Equipped,
        DialogueTree NotEquipped
    ) : DialogueBranch(Equipped, NotEquipped);

    //

    public record DialogueContinue();

    public record TimedContinue(float Duration) : DialogueContinue();

    public record ContinueButtonHit() : DialogueContinue();

    public record Choice(List<(string, DialogueTree)> Choices) : DialogueContinue();

    public record NarrativeCharacter(string Name, Color Color) { }
}
