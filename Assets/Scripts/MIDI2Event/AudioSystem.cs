using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static MIDI2EventUnity;

public class AudioSystem : MonoBehaviour
{
    [SerializeField]
    AudioMixer Mixer;

    [SerializeField]
    List<MIDI2EventUnity> Songs;

    [SerializeField]
    MIDI2EventUnity StartingSong;

    [SerializeField]
    float fadeTime = 1;

    MIDI2EventUnity current;
    public MIDI2EventUnity CurrentSystem
    {
        get => current;
    }

    // Start is called before the first frame update
    void Start()
    {
        current = StartingSong;
    }

    public void SwitchToSong(int index)
    {
        if (index >= 0 && index < Songs.Count)
        {
            FadeToNext(current, Songs[index]);
        }
        else
        {
            Debug.Log("Song index out of bounds!");
        }
    }

    public void FadeOut()
    {
        FadeToNext(current, null);
    }

    IEnumerable FadeToNext(MIDI2EventUnity previous, MIDI2EventUnity next)
    {
        if (previous == null)
        {
            StartNextIfRelevant(next);
            yield break;
        }

        float elapsedTime = 0;
        Mixer.GetFloat(previous.VolumeSliderName, out float startingVol);
        while (elapsedTime < fadeTime)
        {
            float volStep = Mathf.Lerp(startingVol, 0, elapsedTime / fadeTime);
            Mixer.SetFloat(previous.VolumeSliderName, volStep);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        previous.Stop();
        StartNextIfRelevant(next);
        Mixer.SetFloat(previous.VolumeSliderName, startingVol);
    }

    void StartNextIfRelevant(MIDI2EventUnity toStart)
    {
        if (toStart != null)
        {
            toStart.Play();
        }
        current = toStart;
    }
}
