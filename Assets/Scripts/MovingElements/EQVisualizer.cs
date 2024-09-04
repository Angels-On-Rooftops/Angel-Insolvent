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
    float AmpMult = 10;

    [SerializeField]
    [Range(6, 13)]
    int SampleArrayPower = 6;

    [SerializeField]
    FFTWindow WindowType = FFTWindow.Rectangular;

    float[] Spectrum;
    GameObject[] BinObjects;

    // Start is called before the first frame update
    void Start()
    {
        Spectrum = new float[(int)Mathf.Pow(2, SampleArrayPower)];
        BinObjects = new GameObject[NumBins];
        MakeBins();
    }

    float timer = 0;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer < 0.05f)
        {
            return;
        }
        timer = 0;

        Audio.GetSpectrumData(Spectrum, 0, WindowType);
        float[] subSpectrum = new float[64];
        Array.Copy(Spectrum, subSpectrum, 64);
        

        //Debug.Log(string.Join(",", Spectrum));

        foreach(GameObject binVis in BinObjects)
        {
            TransformUtil.AddScaleOneDirection(binVis.transform, -Vector3.up * binVis.transform.localScale.y);
        }

        for (int i = 0; i < subSpectrum.Length; i++)
        {
            int bin = i;
            TransformUtil.AddScaleOneDirection(BinObjects[bin].transform, AmpMult * subSpectrum[i] * Vector3.up);
            /*
                BinObjects[bin].transform.localScale += Vector3.up * Spectrum[i] * AmpMult;
            if(BinObjects[bin].transform.localScale.y > BinMax)
            {
                BinObjects[bin].transform.localScale = new Vector3(BinObjects[bin].transform.localScale.x, BinMax, BinObjects[bin].transform.localScale.z);
            }
            */
        }
    }

    void MakeBins()
    {
        float EQWidth = ScaleRef.transform.localScale.x;
        Vector3 instanceScale = new Vector3(EQWidth / NumBins, 0, ScaleRef.transform.localScale.z);
        for (int i = 0; i<NumBins; i++)
        { 
            //create block instance
            GameObject currInstance = Instantiate(EQBlock, transform);

            //offset to correct position
            currInstance.transform.Translate(Vector3.up * ScaleRef.transform.localScale.y / 2);
            currInstance.transform.Translate(Vector3.right * (-EQWidth/2 + EQWidth/NumBins * i + EQWidth/(NumBins*2)));

            //scale to correct size
            currInstance.transform.localScale = instanceScale;

            //add to array
            BinObjects[i] = currInstance;
        }
    }
}
