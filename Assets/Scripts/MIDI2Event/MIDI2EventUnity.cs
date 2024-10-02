using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIDI2EventSystem;
using static MIDI2EventSystem.Midi2Event;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

//a wrapper class to make interfacing with the midi2event easier in the unity editor

public class MIDI2EventUnity : MonoBehaviour
{
    //path to the midi file to use as a chart
    [SerializeField]
    string streamingAssetChartPath;

    //audio source containing audio associated with the chart
    [SerializeField]
    AudioSource audioSource;

    //the lowest octave present in the midi data
    [SerializeField]
    int lowestOctave = -1;

    [SerializeField]
    bool playOnStart;

    Midi2Event eventPlayer;
    float beforeSamples = 0;
    float startTimer = 0.5f;
    bool firstPlay = false;
    float lastTime = 0;

    public Action OnPlay { get; set; }
    public Action OnStop { get; set; }
    public Action OnRestart { get; set; }
    public Action OnEnd { get; set; }

    public float SecPerBeat
    {
        get => (float)eventPlayer.SecPerBeat;
    }

    public float BeatPerSec
    {
        get => (float)eventPlayer.BeatPerSec;
    }

    //returns whether the system is currently playing
    public bool IsPlaying
    {
        get => eventPlayer.IsPlaying;
    }

    void Awake()
    {
        string chartPath = Application.streamingAssetsPath + "/" + streamingAssetChartPath;
        eventPlayer = new(chartPath, lowestOctave);
        audioSource.clip.LoadAudioData();
        OnPlay += () => { };
        OnStop += () => { };
        OnRestart += () => { };
    }

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    //update the event system every frame
    void Update()
    {
        /*
        if (startDelayTime > startTimer)
        {
            startTimer += Time.deltaTime;
            return;
        }

        if (!firstPlay)
        {
            firstPlay = true;
            Play();
        }
        */

        if (beforeSamples > audioSource.timeSamples)
        {
            //audio has ended/looped
            if (!audioSource.loop)
            {
                return;
            }
            eventPlayer.Back();
            eventPlayer.Play();
            lastTime = 0;
        }
        beforeSamples = audioSource.timeSamples;

        eventPlayer.Update(audioSource.time - lastTime);
        lastTime = audioSource.time;
    }

    //plays the audio and chart
    public void Play()
    {
        eventPlayer.Play();
        audioSource.Play();
        OnPlay.Invoke();
    }

    //stops the audio and chart from playing and resets them to the beginning
    public void Stop()
    {
        eventPlayer.Stop();
        audioSource.Stop();
        OnStop.Invoke();
    }

    public void Restart()
    {
        Stop();
        Play();
    }

    /*
     *  subscribes an action to trigger when the specific event occurs in the midi file
     *  returns a function which unsubscribes the action from the event
     *
     *  params:
     *  action - the action to subscribe to the event system
     *  note - the midi note of the event to subscribe to
     *  octave - the octave of the note in the midi chart
     *  type - the type of subscription to use (start of note, end of note, end of chart)
     */
    public Action Subscribe(
        Action action,
        Notes note = 0,
        int octave = 0,
        SubType type = SubType.Start
    )
    {
        return eventPlayer.Subscribe(action, note, octave, type);
    }
}
