using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class MinimalTransform
{
    public Vector3 Position;
    public Quaternion Rotation;

    public MinimalTransform(Transform fromTransform)
        : this(fromTransform.position, fromTransform.rotation) { }

    public MinimalTransform(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}
