using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPositionOnSpring : MonoBehaviour
{
    [SerializeField]
    Transform ToFollow;

    [SerializeField]
    Vector3 PositionSmoothing = new Vector3(2, 5, 2);

    float xVelocity = 0;
    float yVelocity = 0;
    float zVelocity = 0;

    // Update is called once per frame
    void Update()
    {
        if (Time.deltaTime == 0)
        {
            return;
        }
        
        float newX = Mathf.SmoothDamp(
                transform.position.x,
                ToFollow.position.x,
                ref xVelocity,
                PositionSmoothing.x * Time.deltaTime
        ); 
        float newY = Mathf.SmoothDamp(
                transform.position.y,
                ToFollow.position.y,
                ref yVelocity,
                PositionSmoothing.y * Time.deltaTime
        );
        float newZ = 
            Mathf.SmoothDamp(
                transform.position.z,
                ToFollow.position.z,
                ref zVelocity,
                PositionSmoothing.z * Time.deltaTime
        );

        Vector3 newPos = new Vector3(newX, newY, newZ);

        bool didHit = Physics.Raycast(
            ToFollow.position,
            newPos - ToFollow.position,
            out RaycastHit hit,
            (newPos - ToFollow.position).magnitude,
            ControlConstants.RAYCAST_MASK,
            QueryTriggerInteraction.Ignore
        );

        if(didHit) {
            Debug.Log("didHit");
            
            //transform.position = ToFollow.position;
        }
        transform.position = newPos;
    }
}
