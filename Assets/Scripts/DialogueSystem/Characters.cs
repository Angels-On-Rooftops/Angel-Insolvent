using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.DialogueSystem
{
    static class Characters
    {
        public static NarrativeCharacter OverheadSpeaker =
            new("Overhead Speaker", new Color(255, 255, 255));
        public static NarrativeCharacter Opus = new("Opus", new Color(255, 255, 255));
        public static NarrativeCharacter Coda = new("Coda", new Color(255, 255, 0));
        public static NarrativeCharacter Clef = new("Coda", new Color(0, 255, 0));
    }
}
