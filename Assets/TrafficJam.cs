using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarLine
{
    public Transform lineParent; // The parent GameObject for this line of cars
    [HideInInspector]
    public Transform[] cars; // Automatically populated with child cars

    public void InitializeCars()
    {
        if (lineParent != null)
        {
            // Get all child transforms (cars) under the lineParent
            cars = new Transform[lineParent.childCount];
            for (int i = 0; i < lineParent.childCount; i++)
            {
                cars[i] = lineParent.GetChild(i);
            }
        }
        else
        {
            Debug.LogWarning("Line parent is not assigned!");
        }
    }
}

public class TrafficJam : MonoBehaviour
{
    public CarLine leftCarLine; // Assign the left line's parent GameObject
    public CarLine rightCarLine; // Assign the right line's parent GameObject
    public float waveAmplitude = 2f; // Amplitude of the sine wave (distance of each car’s movement along the x-axis)
    public float waveFrequency = 1f; // Frequency of the sine wave (speed of the wave movement)
    public float waveSpeed = 1f; // Speed at which the wave moves along the car line
    private Vector3[] initialLeftPositions;
    private Vector3[] initialRightPositions;

    private void Start()
    {
        // Initialize the cars in each line
        leftCarLine.InitializeCars();
        rightCarLine.InitializeCars();

        // Store initial positions for each car
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
        // Move each car in a sine wave pattern along the x-axis
        ApplySineWaveMovement();
    }

    private void ApplySineWaveMovement()
    {
        for (int i = 0; i < leftCarLine.cars.Length; i++)
        {
            // Calculate x-offset based on sine wave for left and right cars
            float xOffset = Mathf.Sin((Time.time * waveSpeed) + (i * waveFrequency)) * waveAmplitude;

            // Apply the offset to each car’s x position to create the wave effect
            leftCarLine.cars[i].position = new Vector3(initialLeftPositions[i].x + xOffset, initialLeftPositions[i].y, initialLeftPositions[i].z);
            rightCarLine.cars[i].position = new Vector3(initialRightPositions[i].x - xOffset, initialRightPositions[i].y, initialRightPositions[i].z);
        }
    }
}