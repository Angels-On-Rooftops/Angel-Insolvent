using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class EQVisualizer : MonoBehaviour
{
    [SerializeField]
    SampleProcessor Processor;

    [SerializeField]
    GameObject EQBlock;

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
    GameObject ScaleRef;

    [SerializeField]
    bool HideScaleRef = true;

    float[] Bins;
    GameObject[] BinObjects;

    // Start is called before the first frame update
    void Start()
    {
        BinObjects = new GameObject[Processor.NumBins];
        MakeBins();
        if (HideScaleRef)
        {
            ScaleRef.SetActive(false);
        }
    }

    float timer = 0;

    private void OnEnable()
    {
        Processor.Updated += UpdateBins;
    }

    private void OnDisable()
    {
        Processor.Updated -= UpdateBins;
    }

    void UpdateBins()
    {
        Bins = Processor.GetProcessedData();
        ResetBins();
        ScaleBins();
    }

    void ResetBins()
    {
        foreach (GameObject binVis in BinObjects)
        {
            TransformUtil.AddScaleOneDirection(
                binVis.transform,
                -Vector3.up * binVis.transform.localScale.y
            );
        }
    }

    void ScaleBins()
    {
        
        for(int binIndex = 0; binIndex<Bins.Length; binIndex++)
        {
            float binVal = Bins[binIndex];

            binVal *= Mathf.Sqrt(AmpMult * binIndex * HighBoost / Processor.GetNumSamplesAtIndex(binIndex));

            //bring value into range between binmin and binmax
            if (binVal > BinMax)
            {
                binVal = BinMax;
            }
            if (binVal < BinMin)
            {
                binVal = BinMin;
            }

            TransformUtil.AddScaleOneDirection(BinObjects[binIndex].transform, binVal * Vector3.up);
        }
        
    }

    void MakeBins()
    {
        float EQWidth = ScaleRef.transform.localScale.x;
        Vector3 instanceScale = new Vector3(EQWidth / Processor.NumBins, 0, ScaleRef.transform.localScale.z);
        Vector3 firstVerticalOffset = Vector3.up * ScaleRef.transform.localScale.y / 2;

        //create first bin obj
        Vector3 firstOffset =
            Vector3.right * (-EQWidth / 2 + EQWidth / (Processor.NumBins * 2)) + firstVerticalOffset;
        BinObjects[0] = MakeBin(Vector3.zero, firstOffset, Quaternion.identity, instanceScale);
        Vector3 prevOffset = BinObjects[0].transform.localPosition;
        Vector3 prevTransContrib = Vector3.right * EQWidth / Processor.NumBins / 2;

        //create remaining bin objects
        for (int i = 1; i < Processor.NumBins; i++)
        {
            Vector3 nextOffsetDir =
                new(
                    Mathf.Cos(Mathf.Deg2Rad * i * AngleBetween),
                    Mathf.Sin(Mathf.Deg2Rad * i * AngleBetween),
                    0
                );
            Vector3 nextOffsetContrib = nextOffsetDir * EQWidth / Processor.NumBins / 2;
            BinObjects[i] = MakeBin(
                prevOffset,
                prevTransContrib + nextOffsetContrib,
                Quaternion.AngleAxis(AngleBetween * i, Vector3.forward),
                instanceScale
            );

            prevOffset = BinObjects[i].transform.localPosition;
            prevTransContrib = nextOffsetContrib;
        }
    }

    GameObject MakeBin(Vector3 basePos, Vector3 posOffset, Quaternion rotation, Vector3 scale)
    {
        //create block instance
        GameObject result = Instantiate(EQBlock, transform);
        result.transform.Translate(basePos);
        result.transform.Translate(posOffset);
        result.transform.localRotation = rotation;
        result.transform.localScale = scale;
        //result.transform.Translate(Vector3.up);

        return result;
    }
}
