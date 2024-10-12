using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicSampler : MonoBehaviour
{
    [SerializeField]
    MIDI2EventUnity Track;

    [SerializeField]
    [Range(6, 13)]
    int SampleArrayPower = 6;

    [SerializeField]
    FFTWindow WindowType = FFTWindow.Hamming;

    [SerializeField]
    float CacheUpdateDelay = 0;

    public Action Updated;

    float timer = 0;
    float[] cache;

    public float DataArrayPower
    {
        get => SampleArrayPower;
    }

    public float[] SampledData
    {
        get => cache;
    }

    // Start is called before the first frame update
    void Start()
    {
        cache = new float[(int)Mathf.Pow(2, SampleArrayPower)];
    }

    // Update is called once per frame
    void Update()
    {
        //get new sample if enough time has passed
        timer += Time.deltaTime;
        if (timer >= CacheUpdateDelay)
        {
            Track.AudioSource.GetSpectrumData(cache, 0, WindowType);
            Updated.Invoke();
            timer = 0;
        }
    }
}
