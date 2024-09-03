using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float BinMin = 0;

    [SerializeField]
    float BinMax = 1;

    [SerializeField]
    [Range(6, 13)]
    int SampleArrayPower = 6;

    [SerializeField]
    FFTWindow WindowType = FFTWindow.Rectangular;

    float[] Spectrum;
    float[] Bins;
    GameObject[] BinObjects;

    // Start is called before the first frame update
    void Start()
    {
        Spectrum = new float[(int)Mathf.Pow(2, SampleArrayPower)];
        Bins = new float[NumBins];
        BinObjects = new GameObject[NumBins];
        MakeBins();
    }

    // Update is called once per frame
    void Update()
    {
        Audio.GetSpectrumData(Spectrum, 0, WindowType);
        //Debug.Log(string.Join(",", Spectrum));
    }

    void MakeBins()
    {
        float EQWidth = ScaleRef.transform.localScale.x;
        Vector3 instanceScale = new Vector3(EQWidth / NumBins, 1, ScaleRef.transform.position.z);
        for (int i = 0; i<Bins.Length; i++)
        {
            

            //create block instance
            GameObject currInstance = Instantiate(EQBlock, transform);

            //offset to correct position
            currInstance.transform.Translate(Vector3.up * ScaleRef.transform.localScale.y / 2);


            Debug.Log(Vector3.right * (-EQWidth / 2 + EQWidth / NumBins + EQWidth / (NumBins * 2)));
            currInstance.transform.Translate(Vector3.right * (-EQWidth/2 + EQWidth/NumBins * i + EQWidth/(NumBins*2)));

        }
    }
}
