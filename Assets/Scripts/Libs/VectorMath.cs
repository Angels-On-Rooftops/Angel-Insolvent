using System;
using UnityEngine;

static class VectorMath
{
    public static Vector3 Apply(Vector3 v, Func<float, float> func) 
    {
        return new Vector3(func(v.x), func(v.y), func(v.z));
    }

    public static Vector3 Sqrt(Vector3 v)
    {
        return Apply(v, Mathf.Sqrt);
    }

    public static Vector3 Abs(Vector3 v)
    {
        return Apply(v, Mathf.Abs);
    }

}
