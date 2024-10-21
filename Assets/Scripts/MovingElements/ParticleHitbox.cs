using MIDI2EventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHitbox : MonoBehaviour
{
    [SerializeField] private MIDI2EventUnity midiEventSys;
    [SerializeField] private ParticleSystem particleSys;
    [Space]
    [SerializeField] private Notes note;
    [SerializeField] private int octave;
    [Space]
    [SerializeField] private Notes indicatorBeatNote;
    [SerializeField] private int indicatorBeatOctave;

    private Action unsubPlay;
    private Action unsubStop;

    // Start is called before the first frame update
    void Start()
    {
        this.particleSys.Stop();
    }

    private void OnEnable()
    {
        unsubPlay = midiEventSys.Subscribe(PlayParticles, note, octave);
        unsubStop = midiEventSys.Subscribe(StopParticles, note, octave, Midi2Event.SubType.End);
    }

    private void OnDisable()
    {
        unsubPlay.Invoke();
        unsubStop.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int i = 0;
    bool playing = false;
    void PlayParticles()
    {
        /*i++;
        if (i >= 10)
        {
            if (playing)
            {
                this.particleSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                playing = false;
            }
            {
                this.particleSys.Play();
                playing = true;
            }
            i = 0;
        }*/
        this.particleSys.Play();
    }

    void StopParticles()
    {
        //this.particleSys.Stop();
        this.particleSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
