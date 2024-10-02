using MIDI2EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Swinger : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity EventSys;

    [SerializeField]
    Notes SwingNote;

    [SerializeField]
    int SwingOctave;

    [SerializeField]
    float Angle;

    [SerializeField]
    float SwingSpeed = 0.2f;

    private Action unsub;

    private void OnEnable()
    {
        unsub = EventSys.Subscribe(Swing, SwingNote, SwingOctave);
    }

    private void OnDisable()
    {
        unsub();
    }

    Vector3 currentEuler;
    Vector3 oppositeEuler;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.Rotate(Vector3.forward, Angle);
        currentEuler = this.transform.localEulerAngles;
        oppositeEuler = currentEuler + Vector3.forward * -2 * Angle;
    }

    void Swing()
    {
        (currentEuler, oppositeEuler) = (oppositeEuler, currentEuler);
        StartCoroutine(SmoothSwing(currentEuler, oppositeEuler));
    }

    IEnumerator SmoothSwing(Vector3 current, Vector3 target)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += SwingSpeed * Time.deltaTime;
            this.transform.localEulerAngles = new Vector3(
                Mathf.SmoothStep(current.x, target.x, progress),
                Mathf.SmoothStep(current.y, target.y, progress),
                Mathf.SmoothStep(current.z, target.z, progress)
            );
            yield return null;
        }
    }
}
