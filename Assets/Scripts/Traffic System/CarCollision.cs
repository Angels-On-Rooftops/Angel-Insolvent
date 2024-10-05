using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    [SerializeField]
    float VisionAngle;

    [SerializeField]
    float Deceleration = 2;

    float defaultSpeed;

    GraphMovement Movement => GetComponent<GraphMovement>();

    private void Start()
    {
        defaultSpeed = Movement.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        float agentToVertexAngle = Vector3.Angle(other.transform.position, transform.position);

        if (agentToVertexAngle < VisionAngle)
        {
            return;
        }

        if (other.gameObject.CompareTag("Person"))
        {
            Movement.speed = 0;
            return;
        }

        if (other.gameObject.CompareTag("Car"))
        {
            float otherSpeed = other.GetComponent<GraphMovement>().speed;
            Movement.speed = Mathf.Max(otherSpeed - Deceleration, 0);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        List<string> carStoppers = new() { "Car", "Person" };

        if (carStoppers.Contains(other.tag))
        {
            Movement.speed = defaultSpeed;
        }
    }
}
