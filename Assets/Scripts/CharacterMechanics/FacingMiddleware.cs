using System;
using System.Collections;
using UnityEngine;


public static class FacingMiddleware
{

    public static Func<Vector3, float, Vector3> FaceWithCamera(CharacterMovement movement)
    {
        return (v, n) => { return Vector3.zero; };
    }
}