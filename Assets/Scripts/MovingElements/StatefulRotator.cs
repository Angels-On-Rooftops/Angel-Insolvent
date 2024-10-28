using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulRotator : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity TrackInfo;

    [SerializeField]
    MIDI2EventSystem.Notes Note;

    [SerializeField]
    int Octave = -2;

    [SerializeField]
    Vector3[] Rotations = { };

    [SerializeField]
    int CurrentRotation = 0;

    void Awake()
    {
        TrackInfo.Subscribe(() => CurrentRotation = (CurrentRotation + 1) % Rotations.Length, Note, Octave);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Rotations[CurrentRotation]);
    }
}
