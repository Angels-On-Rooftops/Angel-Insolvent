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
    List<NoteStringTriplet> BPMForeshadows;

    [SerializeField]
    Notes BPMChangeNote;

    [SerializeField]
    int BPMChangeOctave;

    [SerializeField]
    List<NoteStringTriplet> SignatureForeshadows;

    [SerializeField]
    Notes SignatureChangeNote;

    [SerializeField]
    int SignatureChangeOctave;

    string currentBPMString = "120";
    string currentTimeSignatureString = "4/4";
    string nextBPMString;
    string nextTimeSignatureString;

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
    }

    void AnticipateBPMChange(string upcoming)
    {

    }

    void BPMChange()
    {

    }

    void AnticipateSignatureChange(string upcoming)
    {

    }

    void SignatureChange()
    {

    }

    void SetText(string firstLine, string secondLine)
    {

    }

    [Serializable]
    struct NoteStringTriplet
    {
        public Notes note;
        public int octave;
        public string noteString;
    }

}
