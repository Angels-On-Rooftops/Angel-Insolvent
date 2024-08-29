using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class TimerPlatform : MonoBehaviour
{
    [SerializeField]
    public Material onMaterial;

    private Mesh triangle;
    private RenderParams onParams;

    // Start is called before the first frame update
    void Start()
    {
        onParams = new RenderParams(onMaterial);
        triangle = TriangleMaker.MakeTriangle(
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0)
        );
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.RenderMesh(onParams, triangle, 0, Matrix4x4.identity);
    }
}
