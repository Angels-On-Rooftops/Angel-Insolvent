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

    readonly Maid screenMaid = new();

    private void SetInitVals()
    {
        currentBPMString = StartingBPMString;
        currentSignatureString = StartingSignatureString;
        nextBPMString = StartingBPMString;
        nextSignatureString = StartingSignatureString;
        SetText(currentBPMString, currentSignatureString);
    }

    private void OnEnable()
    {
        foreach (NoteStringTriplet t in BPMForeshadows)
        {
            screenMaid.GiveTask(
                EventSys.Subscribe(() => AnticipateBPMChange(t.noteString), t.note, t.octave)
            );
        }
        screenMaid.GiveTask(EventSys.Subscribe(() => BPMChange(), BPMChangeNote, BPMChangeOctave));

        foreach (NoteStringTriplet t in SignatureForeshadows)
        {
            screenMaid.GiveTask(
                EventSys.Subscribe(() => AnticipateSignatureChange(t.noteString), t.note, t.octave)
            );
        }
        screenMaid.GiveTask(
            EventSys.Subscribe(() => SignatureChange(), SignatureChangeNote, SignatureChangeOctave)
        );

        EventSys.OnPlay += SetInitVals;
        screenMaid.GiveTask(() => EventSys.OnStop -= SetInitVals);

        Action clearScreen = () =>
        {
            SetText("", "");
        };
        EventSys.OnStop += clearScreen;
        screenMaid.GiveTask(() => EventSys.OnStop -= clearScreen);
    }

    private void OnDisable()
    {
        screenMaid.Cleanup();
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
        Debug.Log("here");
    }

    void SignatureChange()
    {
        currentSignatureString = nextSignatureString;
        SetText(currentBPMString, currentSignatureString);
        Debug.Log("here2");
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
