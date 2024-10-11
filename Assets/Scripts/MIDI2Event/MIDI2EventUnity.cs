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
    List<TrackChartInfo> audioInfo;

    [SerializeField]
    bool playOnStart;

    List<Midi2Event> eventPlayers;
    float beforeSamples = 0;
    float lastTime = 0;
    int currentTrackIndex = 0;

    public Action OnPlay { get; set; }
    public Action OnStop { get; set; }
    public Action OnRestart { get; set; }
    public Action OnEnd { get; set; }

    public float SecPerBeat
    {
        get => (float)eventPlayers[currentTrackIndex].SecPerBeat;
    }

    public float BeatPerSec
    {
        get => (float)eventPlayers[currentTrackIndex].BeatPerSec;
    }

    //returns whether the system is currently playing
    public bool IsPlaying
    {
        get => eventPlayers[currentTrackIndex].IsPlaying;
    }

    void Awake()
    {
        eventPlayers = new();
        foreach (TrackChartInfo info in audioInfo)
        {
            string chartPath = Application.streamingAssetsPath + "/" + info.streamingAssetChartPath;
            eventPlayers.Add(new(chartPath, info.lowestOctave));
            info.audioSource.clip.LoadAudioData();
        }

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
        if (beforeSamples > audioInfo[currentTrackIndex].audioSource.timeSamples)
        {
            //audio has ended/looped
            if (!audioInfo[currentTrackIndex].audioSource.loop)
            {
                AdvanceTracks();
                return;
            }

            LoopCurrentTrack();
        }
        UpdateTrackTime();
    }

    private void UpdateTrackTime()
    {
        beforeSamples = audioInfo[currentTrackIndex].audioSource.timeSamples;

        eventPlayers[currentTrackIndex].Update(
            audioInfo[currentTrackIndex].audioSource.time - lastTime
        );
        lastTime = audioInfo[currentTrackIndex].audioSource.time;
    }

    private void LoopCurrentTrack()
    {
        eventPlayers[currentTrackIndex].Back();
        eventPlayers[currentTrackIndex].Play();
        lastTime = 0;
    }

    private void AdvanceTracks()
    {
        eventPlayers[currentTrackIndex].Back();
        //audioInfo[currentTrackIndex].audioSource.Stop();

        currentTrackIndex++;

        eventPlayers[currentTrackIndex].Play();
        //audioInfo[currentTrackIndex].audioSource.Play();
    }

    //plays the audio and chart
    public void Play()
    {
        currentTrackIndex = 0;
        int i = 0;
        double scheduledStartTime = AudioSettings.dspTime;
        audioInfo[i].audioSource.PlayScheduled(scheduledStartTime);
        while (i < audioInfo.Count - 1 && !audioInfo[i].audioSource.loop)
        {
            AudioClip clip = audioInfo[i].audioSource.clip;
            double currentDuration = (double)clip.samples / clip.frequency;

            scheduledStartTime += currentDuration;

            audioInfo[i + 1].audioSource.PlayScheduled(scheduledStartTime);
            i++;
        }
        eventPlayers[currentTrackIndex].Play();
        OnPlay.Invoke();
    }

    //stops the audio and chart from playing and resets them to the beginning
    public void Stop()
    {
        eventPlayers[currentTrackIndex].Stop();
        audioInfo[currentTrackIndex].audioSource.Stop();
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
        Action result = () => { };
        foreach (Midi2Event subsys in eventPlayers)
        {
            result += subsys.Subscribe(action, note, octave, type);
        }
        return result;
    }

    [Serializable]
    internal struct TrackChartInfo
    {
        //path to the midi file to use as a chart
        public string streamingAssetChartPath;

        //audio source containing audio associated with the chart
        public AudioSource audioSource;

        //the lowest octave present in the midi data
        public int lowestOctave;
    }

    internal enum PlayType
    {
        OnceThrough,
        Loop
    }
}
