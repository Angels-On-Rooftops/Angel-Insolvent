using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using MIDI2EventSystem;
using System;

public class TimerPlatform : MonoBehaviour
{
    [SerializeField]
    public Material OnMaterial;

    [SerializeField]
    public float TriangleRadius;

    [SerializeField]
    public float TriangleCenterOffset;

    [SerializeField]
    public Vector3 TriangleOffset;

    [SerializeField]
    [Range(1, 179)]
    public float MaxInternalTriangleAngle = 120;

    [SerializeField]
    public Notes BeatNote;

    [SerializeField]
    public int BeatOctave;

    [SerializeField]
    internal NoteDebounceTriplet[] Debounces;

    private Mesh[] triangles;
    private RenderParams onParams;

    // Start is called before the first frame update
    void Start()
    {
        onParams = new RenderParams(OnMaterial);
        triangles = GenerateTriangleRing(3);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 1; i++)
        {
            Graphics.RenderMesh(onParams, triangles[i], 0, transform.localToWorldMatrix);
        }
    }

    public Mesh[] GenerateTriangleRing(int numTriangles) {
        Mesh[] result = new Mesh[numTriangles];
        float radAngle = 2 * Mathf.PI / numTriangles;

        Vector3 point1 = Vector3.zero + TriangleOffset;

        float halfFarSideLen = Mathf.Sin(radAngle / 2) * TriangleRadius;
        float height = Mathf.Cos(radAngle / 2) * TriangleRadius;

        Vector3 point2 = point1 + new Vector3(-halfFarSideLen, 0, height);
        Vector3 point3 = point1 + new Vector3(halfFarSideLen, 0, height);
        Mesh newMesh = TriangleMaker.MakeTriangle(point1, point2, point3);
        result[0] = newMesh;
        return result;

    }


}

[Serializable]
internal struct NoteDebounceTriplet
{
    public Notes note;
    public int octave;
    public int numTicks;
}


