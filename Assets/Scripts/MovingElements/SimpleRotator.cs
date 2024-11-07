using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity TrackInfo;

    [SerializeField]
    Vector3 RotationAxis = Vector3.right;

    [SerializeField]
    Space AxisSpace = Space.Self;

    [SerializeField]
    float BeatPeriod;

    [SerializeField]
    bool ResetOnStop = true;

    Quaternion startRotation;

    // Start is called before the first frame update
    void Start()
    {
        startRotation = this.transform.rotation;
    }

    private void OnEnable()
    {
        if (ResetOnStop)
        {
            TrackInfo.OnStop += ResetRotation;
        }
    }

    private void OnDisable()
    {
        if (ResetOnStop)
        {
            TrackInfo.OnStop -= ResetRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!TrackInfo.IsPlaying)
        {
            return;
        }

        this.transform.Rotate(
            RotationAxis,
            360 * TrackInfo.BeatPerSec * 1 / BeatPeriod * Time.deltaTime,
            AxisSpace
        );
    }

    void ResetRotation()
    {
        this.transform.rotation = startRotation;
    }
}
