using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextToHealth : MonoBehaviour
{
    [SerializeField]
    PlayerHealth PlayerHealth;

    // Update is called once per frame
    void Update()
    {
        GetComponent<TMP_Text>().text = "" + PlayerHealth.Health;
    }
}
