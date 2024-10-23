using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnCollision : MonoBehaviour
{
    [SerializeField] private int damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>()?.DecreaseHealth(this.damageAmount);
        }
    }
}
