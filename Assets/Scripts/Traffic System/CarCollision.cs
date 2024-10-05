using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    [SerializeField]
    GraphMovement car;
    float carSpeed;

    [SerializeField]
    float visionAngle; //radians

    private void Start()
    {
        carSpeed = car.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        float agentToVertexAngle = Vector3.Angle(
            other.gameObject.transform.position,
            car.transform.position
        );

        if (agentToVertexAngle > visionAngle)
        {
            if (other.gameObject.CompareTag("Car"))
            {
                float otherSpeed = other.gameObject.GetComponent<GraphMovement>().speed;
                if (otherSpeed > 0)
                {
                    car.speed = otherSpeed - 2;
                }
                else
                {
                    car.speed = 0;
                }
            }
            else if (other.gameObject.CompareTag("Person"))
            {
                car.speed = 0;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car") || other.gameObject.CompareTag("Person"))
        {
            car.speed = carSpeed;
        }
    }
}
