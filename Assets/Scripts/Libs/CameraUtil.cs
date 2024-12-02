using UnityEngine;

namespace Assets.Scripts.Libs
{
    static class CameraUtil
    {
        public static Camera GetPlayerCamera()
        {
            return GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        }
    }
}
