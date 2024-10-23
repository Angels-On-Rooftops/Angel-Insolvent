using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private int numTimesCollided = 0;

    private void Start()
    {
        this.text.text = "Number of Times Player Collided with " + this.gameObject.name + ": " + this.numTimesCollided;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            numTimesCollided++;
            this.text.text = "Number of Times Player Collided with " + this.gameObject.name + ": " + this.numTimesCollided;
        }
    }
}
