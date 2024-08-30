using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using MIDI2EventSystem;
using System;
using System.Drawing;

public class TimerPlatform : MonoBehaviour
{
    [SerializeField]
    public Material OnMaterial;

    [SerializeField]
    public float TriangleCalculationRadius;

    [SerializeField]
    public float TrianglePushout;

    [SerializeField]
    public Vector3 TriangleCenterOffset;

    [SerializeField]
    [Range(1, 179)]
    public float MaxInternalTriangleAngle = 120;

    [SerializeField]
    public Notes BeatNote;

    [SerializeField]
    public int BeatOctave;

    [SerializeField]
    internal NoteDebounceTriplet[] Debounces;

    private Mesh[][] triangleRings;
    private RenderParams onParams;
    private int activeRing = 0;

    // Start is called before the first frame update
    void Start()
    {
        onParams = new RenderParams(OnMaterial);
        triangleRings = new Mesh[Debounces.Length][];
        for(int i = 0; i<triangleRings.Length; i++)
        {
            triangleRings[i] = GenerateTriangleRing(Debounces[i].numTicks);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activeRing = (activeRing + 1) % triangleRings.Length;
        }
        
        for (int i = 0; i < triangleRings[activeRing].Length; i++)
        {
            Graphics.RenderMesh(onParams, triangleRings[activeRing][i], 0, transform.localToWorldMatrix);
        }
    }

    public Mesh[] GenerateTriangleRing(int numTriangles) {
        Mesh[] result = new Mesh[numTriangles];
        float radAngle = 2 * Mathf.PI / numTriangles;
        float maxRadAngle = MaxInternalTriangleAngle * Mathf.Deg2Rad;
        float angleForTris = Mathf.Min(radAngle, maxRadAngle);

        for (int i = 0; i<numTriangles; i++)
        {
            Vector3[] points = new Vector3[3];

            //generate triangle points
            points[0] = Vector3.zero + TriangleCenterOffset;

            //

            float halfFarSideLen = Mathf.Sin(angleForTris / 2) * TriangleCalculationRadius;
            float height = Mathf.Cos(angleForTris / 2) * TriangleCalculationRadius;

            points[1] = points[0] + new Vector3(-halfFarSideLen, 0, height);
            points[2] = points[0] + new Vector3(halfFarSideLen, 0, height);

            //rotate triangle points
            for (int j = 0; j< 3; j++)
            {
                float rotAngle = radAngle * i;
                float oldX = points[j].x;
                points[j].x = points[j].x * Mathf.Cos(rotAngle) - points[j].z * Mathf.Sin(rotAngle);
                points[j].z = oldX * Mathf.Sin(rotAngle) + points[j].z * Mathf.Cos(rotAngle);
            }

            
            //pushout triangle points
            Vector3 unitPush = (points[1] + points[2]).normalized;
            for (int j = 0; j < 3; j++)
            {
                points[j] += unitPush * TrianglePushout;
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


