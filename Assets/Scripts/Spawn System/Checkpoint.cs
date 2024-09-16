using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false; //stops player from reactivation old check points
    private Vector3 postion; //spawn location

    [SerializeField]
    Vector3 offset;

    public bool Activated
    {
        get { return activated; }
        set { activated = value; }
    }
    public Vector3 Postion
    {
        get { return postion; }
    }

    void Awake()
    {
        postion = offset + this.transform.position;
    }
}
