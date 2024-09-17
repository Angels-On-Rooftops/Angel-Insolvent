using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForthMovement : MonoBehaviour
{
    // TODO make these an array of transforms so students can make platforms that travel every which way.
    [SerializeField]
    Transform beginning;
    [SerializeField]
    Transform ending;

    [SerializeField]
    [Tooltip("How fast the platform moves.")]
    float speed = 1;


    // Update is called once per frame
    void Update()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        float distanceBetween = (beginning.position - ending.position).magnitude;
        float t = (Mathf.Sin(speed/distanceBetween * Time.time) + 1) / 2;
        
        body.MovePosition(Vector3.Lerp(beginning.position, ending.position, t));
        body.MoveRotation(Quaternion.Slerp(beginning.rotation, ending.rotation, t));
    }
}
