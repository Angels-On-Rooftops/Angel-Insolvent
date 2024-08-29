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
        triangles = GenerateTriangleRing(5);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < triangles.Length; i++)
        {
            Graphics.RenderMesh(onParams, triangles[i], 0, transform.localToWorldMatrix);
        }
    }

    public Mesh[] GenerateTriangleRing(int numTriangles) {
        Mesh[] result = new Mesh[numTriangles];
        float radAngle = 2 * Mathf.PI / numTriangles;

        for (int i = 0; i<numTriangles; i++)
        {
            Vector3[] points = new Vector3[3];

            //generate triangle points
            points[0] = Vector3.zero + TriangleOffset;

            float halfFarSideLen = Mathf.Sin(radAngle / 2) * TriangleRadius;
            float height = Mathf.Cos(radAngle / 2) * TriangleRadius;

            points[1] = points[0] + new Vector3(-halfFarSideLen, 0, height);
            points[2] = points[0] + new Vector3(halfFarSideLen, 0, height);

            //rotate triangle points
            
            for(int j = 0; j< 3; j++)
            {
                float rotAngle = radAngle * i;
                float oldX = points[j].x;
                points[j].x = points[j].x * Mathf.Cos(rotAngle) - points[j].z * Mathf.Sin(rotAngle);
                points[j].z = oldX * Mathf.Sin(rotAngle) + points[j].z * Mathf.Cos(rotAngle);
            }
            
            
            foreach (Vector3 point in points)
            {
                Debug.Log(point);
            }

            //make triangle mesh
            Mesh newMesh = TriangleMaker.MakeTriangle(points);
            result[i] = newMesh;
        }

        
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


