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
    [Space]
    [SerializeField] private Notes indicatorBeatNote;
    [SerializeField] private int indicatorBeatOctave;
    [SerializeField] private GameObject parentOfObjectsToRotate;
    [SerializeField] private Vector3 initialRotation;
    [SerializeField] private Vector3 indicationSignalRotation;

    private Vector3 initialHitboxPosition;
    private float distanceTraveled = 0;

    private bool stopWasCalled = false;
    
    private Action unsubPlay;
    private Action unsubStop;
    private Action unsubIndicator;

    // Start is called before the first frame update
    void Start()
    {
        this.particleSys?.Stop();

        if (this.hitbox is not null)
        {
            this.initialHitboxPosition = this.hitbox.transform.position;
            this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y, 0);
        }      

        if (this.adjustParticleSettings)
        {
            SetParticleSettings();
        }

        if (this.parentOfObjectsToRotate is not null)
        {
            SetObjectsToRotation(this.initialRotation);
        }     
    }

    private void OnEnable()
    {
        unsubPlay = midiEventSys.Subscribe(PlayParticles, note, octave);
        unsubStop = midiEventSys.Subscribe(StopParticles, note, octave, Midi2Event.SubType.Stop);
        unsubIndicator = midiEventSys.Subscribe(TurnOnIndication, indicatorBeatNote, indicatorBeatOctave);
    }

    private void OnDisable()
    {
        unsubPlay.Invoke();
        unsubStop.Invoke();
        unsubIndicator.Invoke();
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
        this.particleSys?.Play();

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
        float distanceMovedThisFrame = 0;

        if (this.distanceTraveled < this.distance)
        {
            //Move hitbox center
            Vector3 movementVector = this.hitboxVelocity * Time.deltaTime;
            this.hitbox.transform.Translate(movementVector);

            distanceMovedThisFrame = movementVector.magnitude;
        }

        this.distanceTraveled += distanceMovedThisFrame;

        if (this.distanceTraveled < (this.distance - this.hitbox.transform.localScale.z / 2))
        {
            //Increase hitbox size based on translation
            this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y,
                this.hitbox.transform.localScale.z + distanceMovedThisFrame * 2);           
        }
        else if (this.hitbox.transform.localScale.z > this.endHitboxDepth)
        {
            //Start shrinking the hitbox
            float sizeDecreaseFactor = this.hitboxShrinkSpeed * Time.deltaTime;
            this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y,
                this.hitbox.transform.localScale.z * sizeDecreaseFactor);
        }
    }

    private bool ShouldMoveHitbox()
    {
        //Check if StopParticles was called (should not change the hitbox anymore if so)
        if (this.stopWasCalled || this.hitbox is null)
        {
            return false;
        }

        return (this.distanceTraveled < this.distance)
                    || (this.hitbox.transform.localScale.z > this.endHitboxDepth);
    }

    void StopParticles()
    {
        this.stopWasCalled = true;

        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        this.particleSys?.Stop();

        yield return new WaitForSeconds(this.timeBeforeDisappear);

        //this.particleSys?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Reset();
    }

    private void Reset()
    {
        if (this.hitbox is not null)
        {
            //Reset Hitbox
            this.hitbox.transform.position = this.initialHitboxPosition;
            this.hitbox.transform.localScale = new Vector3(this.hitbox.transform.localScale.x, this.hitbox.transform.localScale.y, 0);
        }
            
        this.distanceTraveled = 0;

        if (this.parentOfObjectsToRotate is not null)
        {
            SetObjectsToRotation(this.initialRotation);
        }

        this.stopWasCalled = false;
    }

    void TurnOnIndication()
    {
        if (this.parentOfObjectsToRotate is not null)
        {
            SetObjectsToRotation(this.indicationSignalRotation);
        }     
    }

    private void SetObjectsToRotation(Vector3 rotationVector)
    {
        Quaternion rotationQuaternion = Quaternion.Euler(rotationVector.x, rotationVector.y, rotationVector.z);

        for (int i = 0; i < this.parentOfObjectsToRotate.transform.childCount; i++)
        {
            GameObject childObject = this.parentOfObjectsToRotate.transform.GetChild(i).gameObject;

            childObject.transform.rotation = rotationQuaternion;
        }
    }
}
