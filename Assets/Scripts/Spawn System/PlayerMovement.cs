using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
public float movementSpeed;

    // Start is called before the first frame update
    void Start() {

        }

    // Update is called once per frame
    void FixedUpdate() {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W)) {
            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed * 2.5f;
            }
        else if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.LeftShift)) {
            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed;
            }
        else if (Input.GetKey(KeyCode.S)) {
            transform.position -= transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed;
            }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) {
            transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * movementSpeed;
            }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) {
            transform.position -= transform.TransformDirection(Vector3.left) * Time.deltaTime * movementSpeed;
            }

        DontDestroyOnLoad(this);
        }

    
    }

