using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class TimerTriangles : MonoBehaviour
{
    [SerializeField]
    Material OnMaterial;

    [SerializeField]
    Material OffMaterial;

    [SerializeField]
    float TriangleCalculationRadius;

    [SerializeField]
    float TrianglePushout;

    [SerializeField]
    Vector3 TriangleCenterOffset;

    [SerializeField]
    [Range(1, 179)]
    float MaxInternalTriangleAngle = 120;

    private Mesh[][] TriangleRings;
    private RenderParams onParams;
    private RenderParams offParams;
    private int activeRing = 0;
    private int countdown;

    internal void Initialize(NoteDebounceTriplet[] Debounces)
    {
        onParams = new RenderParams(OnMaterial);
        offParams = new RenderParams(OffMaterial);
        TriangleRings = new Mesh[Debounces.Length][];
        for (int i = 0; i < TriangleRings.Length; i++)
        {
            TriangleRings[i] = GenerateTriangleRing(Debounces[i].numTicks - 1);
        }
    }

    void LateUpdate()
    {
        //render current indicator
        for (int i = 0; i < TriangleRings[activeRing].Length; i++)
        {
            RenderParams toUse = i < countdown ? onParams : offParams;
            Graphics.RenderMesh(
                toUse,
                TriangleRings[activeRing][i],
                0,
                transform.localToWorldMatrix
            );
        }
    }

    public void SetActiveRing(int i)
    {
        activeRing = i;
    }

    public void SetCountdown(int i)
    {
        countdown = i;
    }

    public Mesh[] GenerateTriangleRing(int numTriangles)
    {
        Mesh[] result = new Mesh[numTriangles];
        float radAngle = 2 * Mathf.PI / numTriangles;
        float maxRadAngle = MaxInternalTriangleAngle * Mathf.Deg2Rad;
        float angleForTris = Mathf.Min(radAngle, maxRadAngle);

        for (int i = 0; i < numTriangles; i++)
        {
            Vector3[] points = new Vector3[3];

            //generate triangle points
            points[0] = Vector3.zero;

            float halfFarSideLen = Mathf.Sin(angleForTris / 2) * TriangleCalculationRadius;
            float height = Mathf.Cos(angleForTris / 2) * TriangleCalculationRadius;

            points[1] = points[0] + new Vector3(-halfFarSideLen, 0, height);
            points[2] = points[0] + new Vector3(halfFarSideLen, 0, height);

            //rotate triangle points
            for (int j = 0; j < 3; j++)
            {
                float rotAngle = radAngle * i;
                float oldX = points[j].x;
                points[j].x = points[j].x * Mathf.Cos(rotAngle) - points[j].z * Mathf.Sin(rotAngle);
                points[j].z = oldX * Mathf.Sin(rotAngle) + points[j].z * Mathf.Cos(rotAngle);
            }

            //pushout triangle points and offset them
            Vector3 unitPush = (points[1] + points[2]).normalized;
            for (int j = 0; j < 3; j++)
            {
                points[j] += unitPush * TrianglePushout;
                points[j] += TriangleCenterOffset;
            }

            //make triangle mesh
            Mesh newMesh = TriangleMaker.MakeTriangle(points);
            result[i] = newMesh;
        }

        return result;
    }
}
