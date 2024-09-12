using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;//stops player from reactivation old check points
    private Vector3 postion; //spawn location

    public bool Activated {
        get {
            return activated;
            }
        set {
            activated = !activated;
            }
        }
    
    public Vector3 Postion{
        get {
            return postion;
            }
        set {
            postion = offset + this.transform.position;
            }
        }
    [SerializeField] Vector3 offset;
    

    
}
