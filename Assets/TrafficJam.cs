using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficJam : MonoBehaviour
{
    public CarLine leftCarLine;
    public CarLine rightCarLine; 
    public float waveAmplitude = 2f; 
    public float waveFrequency = 1f; 
    public float waveSpeed = 1f; 
    private Vector3[] initialLeftPositions;
    private Vector3[] initialRightPositions;

    private void Start()
    {
        leftCarLine.InitializeCars();
        rightCarLine.InitializeCars();

        initialLeftPositions = new Vector3[leftCarLine.cars.Length];
        initialRightPositions = new Vector3[rightCarLine.cars.Length];

        for (int i = 0; i < leftCarLine.cars.Length; i++)
        {
            initialLeftPositions[i] = leftCarLine.cars[i].position;
        }

        for (int i = 0; i < rightCarLine.cars.Length; i++)
        {
            initialRightPositions[i] = rightCarLine.cars[i].position;
        }
    }

    private void Update()
    {
        for (int i = 0; i < leftCarLine.cars.Length; i++)
        {
            float xOffset = Mathf.Sin((Time.time * waveSpeed) + (i * waveFrequency)) * waveAmplitude;

            leftCarLine.cars[i].position = initialLeftPositions[i] + leftCarLine.cars[i].right * xOffset;
        }

        for (int i = 0; i < rightCarLine.cars.Length; i++)
        {
            float xOffset = Mathf.Sin((Time.time * waveSpeed) + (i * waveFrequency)) * waveAmplitude;

            rightCarLine.cars[i].position = initialRightPositions[i] - rightCarLine.cars[i].right * xOffset;
        }
    }
}

[System.Serializable]
public class CarLine
{
    public Transform lineParent;
    [HideInInspector]
    public Transform[] cars;
    public void InitializeCars()
    {
        if (lineParent != null)
        {
            cars = new Transform[lineParent.childCount];
            for (int i = 0; i < lineParent.childCount; i++)
            {
                cars[i] = lineParent.GetChild(i);
            }
        }
        else
        {
            Debug.LogWarning("Missing traffic car line");
        }
    }
}