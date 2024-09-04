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
            controller.Move(backToPosition - controller.transform.position);
            controller.transform.position = backToPosition;
        }
    }
}
