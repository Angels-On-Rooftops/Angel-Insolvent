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
        Debug.Log("Huh?");
        gameObject.SetActive(false);
    }
#endif
}
