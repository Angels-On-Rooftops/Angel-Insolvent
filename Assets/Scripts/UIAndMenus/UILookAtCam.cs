using Assets.Scripts.Libs;
using UnityEngine;

public class UILookAtCam : MonoBehaviour
{
    private void LateUpdate()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - CameraUtil.GetPlayerCamera().transform.position);
        transform.rotation = CameraUtil.GetPlayerCamera().transform.rotation;
    }
}
