using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimerPlatformFields : MonoBehaviour
{
    [SerializeField]
    TimerPlatform platformWithEventSys;

    void Awake()
    {
        TimerPlatform thisPlat = this.GetComponent<TimerPlatform>();
        thisPlat.EventSys = platformWithEventSys.EventSys;
        thisPlat.Debounces = platformWithEventSys.Debounces;
        thisPlat.BeatNote = platformWithEventSys.BeatNote;
        thisPlat.BeatOctave = platformWithEventSys.BeatOctave;
    }
}
