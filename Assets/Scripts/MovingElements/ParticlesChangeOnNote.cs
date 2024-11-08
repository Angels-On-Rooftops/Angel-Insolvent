using MIDI2EventSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlesChangeOnNote : MonoBehaviour
{
    [SerializeField] private MIDI2EventUnity midiEventSys;
    [Space]
    [SerializeField] private Notes note;
    [SerializeField] private int octave;
    [Tooltip("Change this to decrease the frequency of the particle changes; ex. Set this to 3 to update the particles once every 3 of the given notes")]
    [SerializeField] private int numOfNotesBeforeUpdate = 1;
    [Space]
    [SerializeField] private List<float> startSpeeds;

    private int numOfNotesPlayedSinceUpdate = 0;
    private int startSpeedsIndex = 0;
    
    private Action unsub;

    private ParticleSystem particleSys;

    private void Start()
    {
        this.particleSys = GetComponent<ParticleSystem>();
        SetParticleSettings();
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

        SetParticleSettings();
    }

    private void SetParticleSettings()
    {
        if (this.startSpeeds.Count > 0)
        {
            var main = this.particleSys.main;
            main.startSpeed = this.startSpeeds[startSpeedsIndex];

            this.startSpeedsIndex++;
            if (this.startSpeedsIndex >= this.startSpeeds.Count)
            {
                this.startSpeedsIndex = 0;
            }
        }
    }
}
