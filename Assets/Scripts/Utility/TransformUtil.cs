using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utility
{
    public class TransformUtil
    {
        /*
        public static void ScaleOneDirection(Transform transform, Vector3 scalar)
        {
            Vector3 startingScale = transform.localScale;
            transform.localScale = new Vector3(scalar.x * startingScale.x, scalar.y * startingScale.y, scalar.z * startingScale.z);
            transform.position = transform.position + ((scalar - startingScale) / 2);
        }
        */

        public static void AddScaleOneDirection(Transform transform, Vector3 scalar)
        {
            transform.localScale += scalar;
            transform.Translate(scalar / 2);
        }
    }
}
