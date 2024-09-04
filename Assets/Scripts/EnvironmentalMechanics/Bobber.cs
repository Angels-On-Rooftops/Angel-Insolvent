using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    [SerializeField]
    [Range(0,3)]
    float Amplitude = 2;

    [SerializeField]
    [Range(0,1)]
    float Offset = 0;

    [SerializeField]
    [Range(0, 5)]
    float Frequency = 1;

    Vector3 initialPosition;

    void Awake()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        transform.position = initialPosition + Vector3.up * Mathf.Sin((Time.time + Offset) * 2*Mathf.PI*Frequency) * Amplitude/2f;
    }


}
