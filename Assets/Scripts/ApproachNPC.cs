using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachNPC : MonoBehaviour
{
    public GameObject popupPrefab;
    List<GameObject> generatedObjs = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other);
        if (generatedObjs.Count == 0 ) {
            Instantiate(popupPrefab, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            generatedObjs.Add(popupPrefab);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Left collision range");
        // get rid of prefab !
    }

}
