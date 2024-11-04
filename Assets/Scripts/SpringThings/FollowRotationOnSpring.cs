using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotationOnSpring : MonoBehaviour
{
    [SerializeField]
    Transform ToFollow;

    // TODO
    //Vector3 smoothTime = Vector3.one * 0.02f;
    //float xVelocity = 0;
    //float yVelocity = 0;
    //float zVelocity = 0;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = ToFollow.rotation;
    }
}
