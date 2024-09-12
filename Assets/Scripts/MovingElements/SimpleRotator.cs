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
    float BeatPeriod = 4;

    // Start is called before the first frame update
    void Start() { }

    private void OnEnable()
    {
        TrackInfo.OnStop += ResetRotation;
    }

    private void OnDisable()
    {
        TrackInfo.OnStop -= ResetRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TrackInfo.IsPlaying())
        {
            return;
        }

        Quaternion rotation = Quaternion.AngleAxis(
            360 * TrackInfo.BeatPerSec() * 1 / BeatPeriod * Time.deltaTime,
            RotationAxis
        );
        this.transform.rotation *= rotation;
    }

    void ResetRotation()
    {
        this.transform.rotation = Quaternion.identity;
    }
}
