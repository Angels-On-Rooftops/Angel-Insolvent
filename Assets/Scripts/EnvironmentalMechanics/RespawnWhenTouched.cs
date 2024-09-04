using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnWhenTouched : MonoBehaviour
{
    Vector3 backToPosition = new Vector3(5.35290718f, 1.2500124f, 4.65454578f);

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController controller))
        {
            controller.Move(backToPosition - controller.transform.position);
        }
    }
}
