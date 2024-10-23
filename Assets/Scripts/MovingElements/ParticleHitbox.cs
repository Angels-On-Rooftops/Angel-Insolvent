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
    [Space]
    [SerializeField] [Tooltip("Leave empty if a hitbox is not desired")]
    private BoxCollider hitbox;
    [SerializeField][Tooltip("z dimension of the hitbox right before dissipating (minimum z size)")]
    private float endHitboxDepth;
    [SerializeField][Tooltip("Distance of particles/hitbox from initial point before dissipating (maximum distance between the initial position of the center of the hitbox and the final position of the center of the hitbox)")]
    private float distance;
    [SerializeField] private Vector3 hitboxVelocity;
    [SerializeField] private float hitboxShrinkSpeed;
    [SerializeField][Tooltip("Time after chart note ends before disappears (may need to be > 0 to prevent bugs)")]
    private float timeBeforeDisappear = 0.25f;
    [Space]
    [SerializeField][Tooltip("Because several settings can effect particle distance/time, adjusting particle settings through script is optional in case it does not work with your specific particle settings")]
    private bool adjustParticleSettings;

    private Vector3 initialHitboxPosition;
    private float distanceTraveled = 0;

    private bool stopWasCalled = false;
    
    private Action unsubPlay;
    private Action unsubStop;

    // Start is called before the first frame update
    void Start()
    {
        this.particleSys.Stop();
        this.initialHitboxPosition = this.hitbox.transform.position;
        this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y, 0);

        if (this.adjustParticleSettings)
        {
            SetParticleSettings();
        }
    }

    private void OnEnable()
    {
        unsubPlay = midiEventSys.Subscribe(PlayParticles, note, octave);
        unsubStop = midiEventSys.Subscribe(StopParticles, note, octave, Midi2Event.SubType.Stop);
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

    private void SetParticleSettings()
    {
        //TODO
    }

    void PlayParticles()
    {
        this.particleSys.Play();

        StartCoroutine(MoveHitboxCoRoutine());
    }

    IEnumerator MoveHitboxCoRoutine()
    {
        while (ShouldMoveHitbox())
        {
            MoveHitbox();
            yield return null;
        }
    }

    private void MoveHitbox()
    {
        if (this.distanceTraveled < this.distance)
        {
            //Move hitbox center
            Vector3 movementVector = this.hitboxVelocity * Time.deltaTime;
            this.hitbox.transform.Translate(movementVector);

            //Increase hitbox size based on translation
            float distanceMovedThisFrame = movementVector.magnitude;
            this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y,
                this.hitbox.transform.localScale.z + distanceMovedThisFrame * 2);

            //Update this.distanceTraveled
            this.distanceTraveled += distanceMovedThisFrame;
        }
        else if (this.hitbox.size.z > this.endHitboxDepth)
        {
            //Finished moving the hitbox, so start shrinking the hitbox until it's size.z is this.endHitboxDepth
            float sizeDecreaseFactor = 1 - this.hitboxShrinkSpeed * Time.deltaTime;
            this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y,
                this.hitbox.transform.localScale.z * sizeDecreaseFactor);
        }
    }

    private bool ShouldMoveHitbox()
    {
        //Check if StopParticles was called (should not change the hitbox anymore if so)
        if (this.stopWasCalled)
        {
            return false;
        }

        return (this.distanceTraveled < this.distance)
                    || (this.hitbox.transform.localScale.z > this.endHitboxDepth);
    }

    void StopParticles()
    {
        //this.stopWasCalled = true;

        //StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        this.particleSys.Stop();

        yield return new WaitForSeconds(this.timeBeforeDisappear);

        //this.particleSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Reset();

        
    }

    private void Reset()
    {
        //Reset Hitbox
        this.hitbox.transform.position = this.initialHitboxPosition;
        this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y, 0);

        this.distanceTraveled = 0;

        this.stopWasCalled = false;
    }
}
