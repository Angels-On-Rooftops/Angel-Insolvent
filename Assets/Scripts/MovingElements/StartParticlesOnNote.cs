using MIDI2EventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class StartParticlesOnNote : MonoBehaviour
{
    [SerializeField] private MIDI2EventUnity midiEventSys;
    [Space]
    [SerializeField] private Notes note;
    [SerializeField] private int octave;
    [Space]
    [Tooltip("Change this to decrease the frequency of starting the particles; ex. Set this to 3 to start the particles once every 3 of the given notes")]
    [SerializeField] private int numOfNotesBeforeUpdate = 1;
    [Space]
    [Tooltip("Select this to alternate between starting and stopping the particles each time.")]
    [SerializeField] private bool alternateStartingAndStopping = false;

    private int numOfNotesPlayedSinceUpdate = 0;
    private bool wasStarted = false;

    private Action unsub;

    private ParticleSystem particleSys;

    private void Start()
    {
        this.particleSys = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        unsub = midiEventSys.Subscribe(UpdateParticles, note, octave);
    }

    private void OnDisable()
    {
        unsub.Invoke();
    }

    void UpdateParticles()
    {
        this.numOfNotesPlayedSinceUpdate++;
        if (this.numOfNotesPlayedSinceUpdate < this.numOfNotesBeforeUpdate)
        {
            return;
        }
        this.numOfNotesPlayedSinceUpdate = 0;


        if (this.alternateStartingAndStopping && this.wasStarted)
        {
            this.particleSys.Stop();
            this.wasStarted = false;
        }
        else
        {
            this.particleSys.Play();
            this.wasStarted = true;
        }
    }
}
