using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInBuild : MonoBehaviour
{
#if UNITY_EDITOR
    private void Awake() { }
#else
    private void Awake()
    {
        gameObject.SetActive(false);
    }
#endif
}
