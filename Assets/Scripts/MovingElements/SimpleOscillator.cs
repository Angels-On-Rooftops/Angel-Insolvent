using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOscillator : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity TrackInfo;

    [SerializeField]
    int BeatPeriod;

    [SerializeField]
    float Distance;

    [SerializeField]
    Vector3 Direction;

    [SerializeField]
    bool ResetOnStop = true;

    Vector3 extremePos;
    Vector3 extremeNeg;
    Vector3 lastPos;
    Vector3 nextPos;
    float currentPos = 0f;
    float speedMod = 1;

    // Start is called before the first frame update
    void Start()
    {
        //establish bounds
        extremePos = transform.position + Direction.normalized * Distance;
        extremeNeg = transform.position;
        lastPos = transform.position;
    }

    private void OnEnable()
    {
        if (ResetOnStop)
        {
            TrackInfo.OnStop += ResetPos;
        }
    }

    private void OnDisable()
    {
        if (ResetOnStop)
        {
            TrackInfo.OnStop -= ResetPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!TrackInfo.IsPlaying)
        {
            return;
        }
        //calc speed
        float posMod = speedMod * TrackInfo.BeatPerSec * 1 / BeatPeriod * Time.deltaTime;

        //smoothly interpolate next position
        currentPos = Mathf.Clamp(currentPos + posMod, 0, 1);
        nextPos = new Vector3(
            Mathf.SmoothStep(extremeNeg.x, extremePos.x, currentPos),
            Mathf.SmoothStep(extremeNeg.y, extremePos.y, currentPos),
            Mathf.SmoothStep(extremeNeg.z, extremePos.z, currentPos)
        );
        //move by difference between current and next position
        transform.position += (nextPos - lastPos);
        lastPos = nextPos;
        //check for direction inversion
        if (currentPos % 1 == 0)
        {
            speedMod *= -1;
        }
    }

    void ResetPos()
    {
        currentPos = 0f;
        speedMod = 1;
        transform.position = extremeNeg;
        lastPos = transform.position;
    }
}
