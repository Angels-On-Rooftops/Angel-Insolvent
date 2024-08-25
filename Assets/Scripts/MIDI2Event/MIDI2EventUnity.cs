using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using midi2event;
using static midi2event.Midi2Event;
using System;

public class MIDI2EventUnity : MonoBehaviour
{
    [SerializeField]
    string chartPath;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    bool playOnStart = true;

    [SerializeField]
    float soundDelay = 0.5f;

    Midi2Event eventPlayer;
    bool startPlayed = false;

    void Awake()
    {
        eventPlayer = new(chartPath);
        audioSource.clip.LoadAudioData();
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (playOnStart && !startPlayed && Input.GetKeyDown(KeyCode.Space))
        {
            Play();
            startPlayed = true;
        }
        eventPlayer.Update(Time.deltaTime);
    }

    public void Play()
    {
        eventPlayer.Play();
        audioSource.PlayDelayed(soundDelay);
    }

    public Action Subscribe(
        Action action,
        Notes note = 0,
        int octave = 0,
        SubType type = SubType.Start
    )
    {
        Debug.Log("player: " + (eventPlayer is null));
        return eventPlayer.Subscribe(action, note, octave, type);
    }
}
