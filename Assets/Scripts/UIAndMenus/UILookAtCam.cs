using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtCam : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

}
