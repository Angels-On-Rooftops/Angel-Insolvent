using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    
    public bool activated = false; //stops player from reactivation old check points
    public Vector3 postion; //spawn location
    [SerializeField] int height = 1; //y position to spawn player at

    // Start is called before the first frame update
    void Start()
    {
        postion = new Vector3(this.transform.position.x, height, this.transform.position.z);
    }

    
}
