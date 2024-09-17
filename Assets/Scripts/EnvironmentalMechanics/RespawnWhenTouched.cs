using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnWhenTouched : MonoBehaviour
{
    [SerializeField]
    Vector3 backToPosition;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController controller))
        {
            controller.enabled = false;
            controller.transform.position = backToPosition;
            controller.enabled = true;
        }
    }
}
