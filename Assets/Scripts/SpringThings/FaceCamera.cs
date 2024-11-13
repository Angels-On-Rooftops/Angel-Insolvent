using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Transform CameraTransform => Camera.main.transform;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(CameraTransform, transform.up);
    }
}
