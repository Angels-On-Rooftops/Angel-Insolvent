using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour
{
    [SerializeField] moveObject car;
    private float carSpeed;
    [SerializeField] float visionAngle; //radians
    [SerializeField] float changeSpeed = 2;
    
    private void Start() {
        carSpeed = car.speed;
        }
    private void OnTriggerEnter(Collider other) {
        Vector3 agentToVVertex = car.transform.position - other.gameObject.transform.position;
        agentToVVertex.Normalize();
        if (Vector3.Dot(agentToVVertex, car.transform.forward) > Mathf.Cos(visionAngle)) {
            if (other.gameObject.CompareTag("Car")) {
                float otherSpeed = other.gameObject.GetComponent<moveObject>().speed;
                if (otherSpeed > 0) {
                    car.speed = otherSpeed - changeSpeed;
                    }
                else {
                    car.speed = 0;
                    }
                }
            else if (other.gameObject.CompareTag("Person")) {
                car.speed = 0;
                }
            }
        }
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Person")) {
            car.speed = 0;
            }
        }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Car") || other.gameObject.CompareTag("Person")) {
            car.speed = carSpeed;
            
            }
        }
    }
