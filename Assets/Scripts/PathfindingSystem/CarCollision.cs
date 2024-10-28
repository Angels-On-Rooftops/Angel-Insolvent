using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollision : MonoBehaviour {
    [SerializeField] GraphMovement car;
    private float carSpeed;
    

    private void Start() {
        carSpeed = car.Speed;
        }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Person") || other.gameObject.CompareTag("Car")) {
            car.Speed = 0;
            }
        }
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Person") || other.gameObject.CompareTag("Car")) {
            car.Speed = 0;
            }
        }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Car") || other.gameObject.CompareTag("Person")) {
            car.Speed = carSpeed;

            }
        }
    }
