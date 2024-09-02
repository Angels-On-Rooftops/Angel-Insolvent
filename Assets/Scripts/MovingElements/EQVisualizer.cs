using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EQVisualizer : MonoBehaviour
{
    [SerializeField]
    AudioSource Audio;

    [SerializeField]
    int Bins;

    [SerializeField]
    int BinHeight;

    [SerializeField]
    [Range(6, 13)]
    int SampleArrayPower = 6;

    [SerializeField]
    FFTWindow WindowType = FFTWindow.Rectangular;

    float[] Spectrum;

    // Start is called before the first frame update
    void Start()
    {
        Spectrum = new float[(int)Mathf.Pow(2, SampleArrayPower)];
    }

    // Update is called once per frame
    void Update()
    {
        Audio.GetSpectrumData(Spectrum, 0, WindowType);
        Debug.Log(string.Join(",", Spectrum));
    }
}
