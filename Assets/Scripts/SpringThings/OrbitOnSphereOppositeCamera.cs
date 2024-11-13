using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitOnSphereOppositeCamera : MonoBehaviour
{
    [SerializeField]
    float OrbitRadius = 1;

    Vector3 CameraPosition => Camera.main.transform.position;
    Vector3 ParentPosition => transform.parent.position;

    void Update()
    {
        transform.position =
            ParentPosition - (CameraPosition - ParentPosition).normalized * OrbitRadius;
    }
}
