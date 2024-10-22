using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Profiling;

public class SampleProcessor : MonoBehaviour
{
    [SerializeField]
    MusicSampler Sampler;

    [SerializeField]
    int numBins;
    public int NumBins
    {
        get => numBins;
    }

    int[] SamplesPerBin;
    float[] Bins;

    public Action Updated;

    public float[] ProcessedData
    {
        get => Bins;
    }

    void Start()
    {
        Bins = new float[NumBins];
        CalcSamplesPerBin();

        Updated += () => { };
    }

    void OnEnable()
    {
        Sampler.Updated += SampleAndProcess;
    }

    void OnDisable()
    {
        Sampler.Updated -= SampleAndProcess;
    }

    void CalcSamplesPerBin()
    {
        SamplesPerBin = new int[NumBins];
        for (int i = 0; i < SamplesPerBin.Length; i++)
        {
            //determine the number of samples to consolidate into this bin
            int samplesPerBin = (int)(
                Mathf.Pow(2, i) * Mathf.Pow(2, Sampler.DataArrayPower - NumBins)
            );

            //each bin must have at least one sample
            if (samplesPerBin < 1)
            {
                samplesPerBin = 1;
            }
            SamplesPerBin[i] = samplesPerBin;
        }
    }

    public int GetNumSamplesAtIndex(int index)
    {
        if (index < 0 || index > SamplesPerBin.Length)
        {
            return 0;
        }
        return SamplesPerBin[index];
    }

    void SampleAndProcess()
    {
        float[] samples = Sampler.SampledData;

        int sampleIndex = 0;
        int nextCap = 0;

        for (int binIndex = 0; binIndex < Bins.Length; binIndex++)
        {
            //decide next index to stop at
            nextCap += SamplesPerBin[binIndex];
            //cant have index beyond bounds of array
            if (nextCap > samples.Length)
            {
                nextCap = samples.Length;
            }

            //consolidate samples into bin
            float binVal = 0;
            while (sampleIndex < nextCap)
            {
                binVal += samples[sampleIndex];
                sampleIndex++;
            }

            //store bin value
            Bins[binIndex] = binVal;
        }
        Updated.Invoke();
    }
}
