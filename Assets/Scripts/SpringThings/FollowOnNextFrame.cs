using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowOnNextFrame : MonoBehaviour
{
    [SerializeField]
    Transform ToFollow;

    Vector3 LastPosition;
    Quaternion LastRotation;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLast();
    }

    void UpdateLast()
    {
        LastPosition = ToFollow.position;
        LastRotation = ToFollow.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(LastPosition, LastRotation);
        UpdateLast();
    }
}
