using System;
using System.Collections;
using System.Collections.Generic;
using MIDI2EventSystem;
using TMPro;
using UnityEngine;

public class TimeSigScreen : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity EventSys;

    [SerializeField]
    TextMeshPro Text;

    [SerializeField]
    string StartingBPMString;

    [SerializeField]
    List<NoteStringTriplet> BPMForeshadows;

    [SerializeField]
    Notes BPMChangeNote;

    [SerializeField]
    int BPMChangeOctave;

    [SerializeField]
    string StartingSignatureString;

    [SerializeField]
    List<NoteStringTriplet> SignatureForeshadows;

    [SerializeField]
    Notes SignatureChangeNote;

    [SerializeField]
    int SignatureChangeOctave;

    string currentBPMString;
    string currentSignatureString;
    string nextBPMString;
    string nextSignatureString;

    void Start()
    {
        foreach(NoteStringTriplet t in BPMForeshadows)
        {
            EventSys.Subscribe(() => AnticipateBPMChange(t.noteString), t.note, t.octave);
        }
        EventSys.Subscribe(() => BPMChange(), BPMChangeNote, BPMChangeOctave);

        foreach (NoteStringTriplet t in SignatureForeshadows)
        {
            EventSys.Subscribe(() => AnticipateSignatureChange(t.noteString), t.note, t.octave);
        }
        EventSys.Subscribe(() => SignatureChange(), SignatureChangeNote, SignatureChangeOctave);

        currentBPMString = StartingBPMString;
        currentSignatureString = StartingSignatureString;
        nextBPMString = StartingBPMString;
        nextSignatureString = StartingSignatureString;
    }

    void AnticipateBPMChange(string upcoming)
    {
        nextBPMString = upcoming;
        currentBPMString += " -> " + upcoming;
        SetText(currentBPMString, currentSignatureString);
    }

    void BPMChange()
    {
        currentBPMString = nextBPMString;
        SetText(currentBPMString, currentSignatureString);
    }

    void AnticipateSignatureChange(string upcoming)
    {
        nextSignatureString = upcoming;
        currentSignatureString += " -> " + upcoming;
        SetText(currentBPMString, currentSignatureString);
    }

    void SignatureChange()
    {
        currentSignatureString = nextSignatureString;
        SetText(currentBPMString, currentSignatureString);
    }

    void SetText(string firstLine, string secondLine)
    {
        Text.text = (firstLine + "<br>" + secondLine);
    }

    [Serializable]
    struct NoteStringTriplet
    {
        public Notes note;
        public int octave;
        public string noteString;
    }

}
