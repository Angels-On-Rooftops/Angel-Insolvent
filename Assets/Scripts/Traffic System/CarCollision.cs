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

    GraphMovement Movement => GetComponentInParent<GraphMovement>();

    private void Start()
    {
        defaultSpeed = Movement.speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 toAngentFromCar = other.transform.position - transform.position;
        Vector3 carForward = transform.forward;
        float angleFromFrontOfCar = Vector3.Angle(toAngentFromCar, carForward);

        if (angleFromFrontOfCar < VisionAngle / 2)
        {
            Movement.speed = 0;
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

        // TODO: does not work because there could still be another person or car in the collider
        // should instead check that all colliders have left
        if (carStoppers.Contains(other.tag))
        {
            Movement.speed = defaultSpeed;
        }
    }
}
