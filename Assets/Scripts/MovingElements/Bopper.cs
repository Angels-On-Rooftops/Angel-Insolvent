using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIDI2EventSystem;
using System;
using Utility;

public class Bopper : MonoBehaviour
{
    [SerializeField]
    float max = 3;

    [SerializeField]
    float decayRate = 0.5f;

    [SerializeField]
    Vector3 direction = Vector3.up;

    [SerializeField]
    MIDI2EventUnity handler;

    [SerializeField]
    Notes note;

    [SerializeField]
    int octave;

    Vector3 startingPos;
    Vector3 startingScale;
    bool needsDecay = false;
    Action unsub;

    void Awake()
    {
        this.startingPos = transform.position;
        this.startingScale = transform.localScale;
    }

    private void OnEnable()
    {
        unsub = handler.Subscribe(Max, note, octave);
    }

    private void OnDisable()
    {
        unsub.Invoke();
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (needsDecay)
        {
            Decay();
        }
    }

    void Max()
    {
        TransformUtil.AddScaleOneDirection(this.transform, (direction * max) - Vector3.Project(startingScale, direction));
        //transform.localScale = new Vector3(startingScale.x, max * startingScale.y, startingScale.z);
        //transform.position = startingPos + Vector3.up * ((max - startingScale.y) / 2);
        needsDecay = true;
    }

    void Decay()
    {
        Vector3 decayVector = direction * Time.deltaTime * decayRate;
        TransformUtil.AddScaleOneDirection(this.transform, -decayVector);
        //transform.localScale = transform.localScale - decayVector;
        //transform.position = transform.position - decayVector * 0.5f;
        if (transform.localScale.magnitude < startingScale.magnitude)
        {
            transform.localScale = startingScale;
            transform.position = startingPos;
            needsDecay = false;
        }
    }
}
