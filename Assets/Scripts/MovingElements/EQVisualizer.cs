using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class EQVisualizer : MonoBehaviour
{
    [SerializeField]
    AudioSource Audio;

    [SerializeField]
    GameObject EQBlock;

    [SerializeField]
    GameObject ScaleRef;

    [SerializeField]
    int NumBins;

    [SerializeField]
    float BinMax = 5;

    [SerializeField]
    float BinMin = 0.1f;

    [SerializeField]
    float AngleBetween = -10f;

    [SerializeField]
    float AmpMult = 10;

    [SerializeField]
    float HighBoost = 1;

    [SerializeField]
    [Range(6, 13)]
    int SampleArrayPower = 6;

    [SerializeField]
    bool HideScaleRef = true;

    [SerializeField]
    FFTWindow WindowType = FFTWindow.Hamming;

    float[] Spectrum;
    GameObject[] BinObjects;

    // Start is called before the first frame update
    void Start()
    {
        Spectrum = new float[(int)Mathf.Pow(2, SampleArrayPower)];
        BinObjects = new GameObject[NumBins];
        MakeBins();
        if (HideScaleRef)
        {
            ScaleRef.SetActive(false);
        }
    }

    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < 0.05f)
        {
            return;
        }
        timer = 0;

        Audio.GetSpectrumData(Spectrum, 0, WindowType);

        foreach (GameObject binVis in BinObjects)
        {
            TransformUtil.AddScaleOneDirection(
                binVis.transform,
                -Vector3.up * binVis.transform.localScale.y
            );
        }
        Debug.Log(Spectrum.Length + ", " + BinObjects.Length);
        int sampleIndex = 0;
        int nextCap = 0;

        for (int binIndex = 0; binIndex < BinObjects.Length; binIndex++)
        {
            int samplesPerBin = (int)(
                Mathf.Pow(2, binIndex) * Mathf.Pow(2, SampleArrayPower - NumBins)
            );

            if (samplesPerBin < 1)
            {
                samplesPerBin = 1;
            }

            nextCap += samplesPerBin;

            if (nextCap > Spectrum.Length)
            {
                nextCap = Spectrum.Length;
            }

            float scale = 0;
            while (sampleIndex < nextCap)
            {
                scale += Spectrum[sampleIndex];
                sampleIndex++;
            }

            scale *= Mathf.Sqrt(AmpMult * binIndex * HighBoost / samplesPerBin);

            if (scale > BinMax)
            {
                scale = BinMax;
            }
            if (scale < BinMin)
            {
                scale = BinMin;
            }

            TransformUtil.AddScaleOneDirection(BinObjects[binIndex].transform, scale * Vector3.up);
        }
        Debug.Log(sampleIndex);
    }

    void MakeBins()
    {
        float EQWidth = ScaleRef.transform.localScale.x;
        Vector3 instanceScale = new Vector3(EQWidth / NumBins, 0, ScaleRef.transform.localScale.z);
        Vector3 firstVerticalOffset = Vector3.up * ScaleRef.transform.localScale.y / 2;

        //create first bin obj
        Vector3 firstOffset =
            Vector3.right * (-EQWidth / 2 + EQWidth / (NumBins * 2)) + firstVerticalOffset;
        BinObjects[0] = MakeBin(Vector3.zero, firstOffset, Quaternion.identity, instanceScale);
        Vector3 prevOffset = BinObjects[0].transform.localPosition;
        Vector3 prevTransContrib = Vector3.right * EQWidth / NumBins / 2;
        for (int i = 1; i < NumBins; i++)
        {
            Vector3 nextOffsetDir =
                new(
                    Mathf.Cos(Mathf.Deg2Rad * i * AngleBetween),
                    Mathf.Sin(Mathf.Deg2Rad * i * AngleBetween),
                    0
                );
            Debug.Log(
                "prev trans component: "
                    + (
                        transform.rotation
                        * BinObjects[i - 1].transform.right
                        * EQWidth
                        / NumBins
                        / 2
                    )
            );
            Debug.Log("calc next component: " + nextOffsetDir * EQWidth / NumBins / 2);
            BinObjects[i] = MakeBin(
                prevOffset,
                prevTransContrib + nextOffsetDir * EQWidth / NumBins / 2,
                Quaternion.AngleAxis(AngleBetween * i, Vector3.forward),
                instanceScale
            );

            prevOffset = BinObjects[i].transform.localPosition;
            prevTransContrib = nextOffsetDir * EQWidth / NumBins / 2;
        }
    }

    GameObject MakeBin(Vector3 basePos, Vector3 posOffset, Quaternion rotation, Vector3 scale)
    {
        //create block instance
        GameObject result = Instantiate(EQBlock, transform);
        result.transform.Translate(basePos);
        Debug.Log("Offset:" + posOffset);
        Debug.Log("Pos b4 offset: " + result.transform.localPosition);
        result.transform.Translate(posOffset);
        Debug.Log("Pos after offset: " + result.transform.localPosition);
        result.transform.localRotation = rotation;
        result.transform.localScale = scale;
        //result.transform.Translate(Vector3.up);

        return result;
    }
}
