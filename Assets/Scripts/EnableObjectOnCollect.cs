using Items.Collectables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectOnCollect : MonoBehaviour
{
    [SerializeField]
    GameObject ObjectToEnable;

    void Awake()
    {
        GetComponent<Collectable>().OnCollection += (GameObject obj) =>
        {
            ObjectToEnable.SetActive(true);
        };
    }
}
