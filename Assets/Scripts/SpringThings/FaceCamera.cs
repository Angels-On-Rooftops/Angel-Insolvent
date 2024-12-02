using Assets.Scripts.Libs;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Transform CameraTransform => CameraUtil.GetPlayerCamera().transform;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(CameraTransform, transform.up);
    }
}
