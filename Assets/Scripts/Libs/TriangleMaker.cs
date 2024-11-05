using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class TriangleMaker
    {
        public static Mesh MakeTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Mesh result = new Mesh();
            result.vertices = new Vector3[] { vertex1, vertex2, vertex3 };
            //TODO maybe do projection shit to make this work for non-horizontal cases
            result.uv = new Vector2[]
            {
                new Vector2(vertex1.x, vertex1.z),
                new Vector2(vertex2.x, vertex2.z),
                new Vector2(vertex3.x, vertex3.z)
            };
            result.triangles = new int[] { 0, 1, 2 };
            Vector3 normal = Vector3.Cross(vertex1-vertex2, vertex2-vertex3);
            result.normals = new Vector3[] { normal, normal, normal };
            return result;
        }

        public static Mesh MakeTriangle(Vector3[] vertecies)
        {
            if (vertecies.Length < 3)
            {
                Debug.Log("triangle missing vertecies!");
                return new Mesh();
            }
            return MakeTriangle(vertecies[0], vertecies[1], vertecies[2]);
        }
    }
}
