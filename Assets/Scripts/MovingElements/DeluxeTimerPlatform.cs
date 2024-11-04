using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MIDI2EventSystem;
using UnityEngine;
using Utility;

public class DeluxeTimerPlatform : MonoBehaviour
{
    [SerializeField]
    internal MIDI2EventUnity EventSys;

    [SerializeField]
    Transform[] TransformLoop;

    [SerializeField]
    internal Notes BeatNote;

    [SerializeField]
    internal int BeatOctave;

    [SerializeField]
    internal NoteDebounceTriplet[] Debounces;

    [SerializeField]
    TimerTriangles[] Timers;

    readonly Maid MusicalMaid = new();

    int countdown;
    private int transformIndex = 0;

    void Start()
    {
        foreach (TimerTriangles t in Timers)
        {
            t.Initialize(Debounces);
        }
        SetAllCountdowns(Debounces[0].numTicks);
    }

    private void OnEnable()
    {
        //subscribe ring setting events
        for (int i = 0; i < Debounces.Length; i++)
        {
            int ringIndex = i;
            MusicalMaid.GiveTask(
                EventSys.Subscribe(
                    () =>
                    {
                        SetAllActiveRing(ringIndex);
                    },
                    Debounces[ringIndex].note,
                    Debounces[ringIndex].octave
                )
            );
        }

        //subscribe beat event
        MusicalMaid.GiveTask(EventSys.Subscribe(DecrementCounter, BeatNote, BeatOctave));
    }

    private void OnDisable()
    {
        MusicalMaid.Cleanup();
    }

    public void SetAllActiveRing(int ringIndex)
    {
        SetAllCountdowns(Debounces[ringIndex].numTicks-1);
        foreach (TimerTriangles t in Timers)
        {
            t.SetActiveRing(ringIndex);
        }
    }

    void SetAllCountdowns(int countdownVal)
    {
        countdown = countdownVal;
        foreach (TimerTriangles t in Timers)
        {
            t.SetCountdown(countdown);
        }
    }

    public void DecrementCounter()
    {
        SetAllCountdowns(countdown - 1);
        //move position if necessary
        if (countdown == 0 && TransformLoop.Length > 0)
        {
            transformIndex = (transformIndex + 1) % TransformLoop.Length;
            StartCoroutine(MoveToTransformOnBeat(TransformLoop[transformIndex]));
        }
    }

    IEnumerator MoveToTransformOnBeat(Transform target)
    {
        float timer = 0;
        float moveDuration = EventSys.SecPerBeat;
        Vector3 startPos = this.transform.position;

        while (timer <= moveDuration)
        {
            this.transform.position = Vector3.Lerp(
                startPos,
                target.position,
                Mathf.SmoothStep(0, 1, timer / moveDuration)
            );
            timer += Time.deltaTime;
            yield return null;
        }

        this.transform.position = target.position;
    }

    
}
