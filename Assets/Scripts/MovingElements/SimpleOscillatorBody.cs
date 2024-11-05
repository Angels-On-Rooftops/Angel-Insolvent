using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleOscillatorBody : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity TrackInfo;

    [SerializeField]
    int BeatPeriod;

    [SerializeField]
    Vector3 Direction;

    [SerializeField]
    bool ResetOnStop = true;

    [SerializeField]
    Transform t1;

    [SerializeField]
    Transform t2;

    MinimalTransform StartingPoint;
    Rigidbody Rigidbody => GetComponent<Rigidbody>();

    Transform To;
    Transform From;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        StartingPoint = new(transform);
        (To, From) = (t1, t2);
    }

    private void OnEnable()
    {
        TrackInfo.OnStop += ResetPos;
    }

    private void OnDisable()
    {
        TrackInfo.OnStop -= ResetPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TrackInfo.IsPlaying)
        {
            return;
        }

        float speed = TrackInfo.BeatPerSec * 1 / BeatPeriod * Time.deltaTime;
        t = Mathf.Clamp(t + speed, 0, 1);
        Rigidbody.MovePosition(Vector3.Lerp(To.position, From.position, t));

        //check for direction inversion
        if (t == 1)
        {
            (To, From) = (From, To);
            t = 0;
        }
    }

    void ResetPos()
    {
        if (!ResetOnStop)
        {
            return;
        }

        transform.SetPositionAndRotation(StartingPoint.Position, StartingPoint.Rotation);
    }
}
