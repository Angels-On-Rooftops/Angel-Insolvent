using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using midi2event;
using System;

public class Bopper : MonoBehaviour
{
    [SerializeField]
    float max = 3;

    [SerializeField]
    float decayRate = 0.5f;

    [SerializeField]
    MIDI2EventUnity handler;

    [SerializeField]
    Midi2Event.Notes note;

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
        Debug.Log("enabling bopper");
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
        transform.localScale = new Vector3(startingScale.x, max * startingScale.y, startingScale.z);
        transform.position = startingPos + Vector3.up * ((max - startingScale.y) / 2);
        needsDecay = true;
    }

    void Decay()
    {
        Vector3 decayVector = Vector3.up * Time.deltaTime * decayRate;
        transform.localScale = transform.localScale - decayVector;
        transform.position = transform.position - decayVector * 0.5f;
        if (transform.localScale.y < startingScale.y)
        {
            transform.localScale = startingScale;
            transform.position = startingPos;
        }
    }
}
