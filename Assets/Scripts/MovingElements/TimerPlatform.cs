using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MIDI2EventSystem;
using UnityEngine;
using Utility;

public class TimerPlatform : MonoBehaviour
{
    [SerializeField]
    internal MIDI2EventUnity EventSys;

    [SerializeField]
    Vector3[] PositionLoop;

    [SerializeField]
    bool LocalMode = false;

    [SerializeField]
    internal Notes BeatNote;

    [SerializeField]
    internal int BeatOctave;

    [SerializeField]
    internal NoteDebounceTriplet[] Debounces;

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
    private int locationIndex = 0;
    private Action[] UnsubActions;

    //private Space space = Space.World; TODO

    void Start()
    {
        onParams = new RenderParams(OnMaterial);
        offParams = new RenderParams(OffMaterial);
        TriangleRings = new Mesh[Debounces.Length][];
        for (int i = 0; i < TriangleRings.Length; i++)
        {
            TriangleRings[i] = GenerateTriangleRing(Debounces[i].numTicks - 1);
        }
        countdown = TriangleRings[activeRing].Length;

        if (LocalMode)
        {
            TriangleCenterOffset = Vector3.Scale(TriangleCenterOffset, transform.lossyScale);
            for (int i = 0; i < PositionLoop.Length; i++)
            {
                PositionLoop[i] = transform.TransformPoint(PositionLoop[i]);
            }
        }
    }

    private void OnEnable()
    {
        UnsubActions = new Action[Debounces.Length + 1];

        //subscribe ring setting events
        for (int i = 0; i < Debounces.Length; i++)
        {
            int ringIndex = i;
            Action switchAction = () =>
            {
                SetActiveRing(ringIndex);
            };
            UnsubActions[i] = EventSys.Subscribe(
                switchAction,
                Debounces[i].note,
                Debounces[i].octave
            );
        }

        //subscribe beat event
        UnsubActions[Debounces.Length] = EventSys.Subscribe(DecrementCounter, BeatNote, BeatOctave);
    }

    private void OnDisable()
    {
        for (int i = 0; i < UnsubActions.Length; i++)
        {
            UnsubActions[i]();
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

    public void DecrementCounter()
    {
        countdown--;
        //move position if necessary
        if (countdown == 0 && PositionLoop.Length > 0)
        {
            locationIndex = (locationIndex + 1) % PositionLoop.Length;
            StartCoroutine(MoveToPosInBeat(PositionLoop[locationIndex]));
        }
    }

    public void SetActiveRing(int i)
    {
        activeRing = i;
        countdown = TriangleRings[i].Length;
    }

    IEnumerator MoveToPosInBeat(Vector3 target)
    {
        float timer = 0;
        float moveDuration = EventSys.SecPerBeat;
        Vector3 startPos = this.transform.position;

        while (timer <= moveDuration)
        {
            this.transform.position = Vector3.Lerp(
                startPos,
                target,
                Mathf.SmoothStep(0, 1, timer / moveDuration)
            );
            timer += Time.deltaTime;
            yield return null;
        }

        this.transform.position = target;
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

[Serializable]
struct NoteDebounceTriplet
{
    public Notes note;
    public int octave;
    public int numTicks;
}
