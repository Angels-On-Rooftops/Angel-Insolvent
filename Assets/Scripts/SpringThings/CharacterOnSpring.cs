using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOnSpring : MonoBehaviour
{
    [SerializeField]
    Transform ToFollow;

    [SerializeField]
    Vector3 PositionSmoothing = new Vector3(2, 5, 2) / 100f;
    float xVelocity = 0;
    float yVelocity = 0;
    float zVelocity = 0;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            Mathf.SmoothDamp(transform.position.x, ToFollow.position.x, ref xVelocity, PositionSmoothing.x),
            Mathf.SmoothDamp(transform.position.y, ToFollow.position.y, ref yVelocity, PositionSmoothing.y),
            Mathf.SmoothDamp(transform.position.z, ToFollow.position.z, ref zVelocity, PositionSmoothing.z)
        );
    }
}
