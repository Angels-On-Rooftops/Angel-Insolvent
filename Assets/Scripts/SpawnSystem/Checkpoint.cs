using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    Vector3 Offset;

    public bool Activated { get; set; }
    public Vector3 RespawnPosition => Offset + transform.position;
}
