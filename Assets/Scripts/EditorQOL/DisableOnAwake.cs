using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAdditiveLoad : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
