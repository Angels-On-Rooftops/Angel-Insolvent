using System.Collections;
using UnityEngine;

public class RemoveMeshOnStart : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}