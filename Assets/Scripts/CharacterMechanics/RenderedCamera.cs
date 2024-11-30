using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderedCamera : MonoBehaviour
{
    [SerializeField]
    CharacterCamera cam;

    void UpdateLockState()
    {
        Cursor.lockState = cam.CanOrbit ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void LateUpdate()
    {
        UpdateLockState();
    }
}
