using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMarker : MonoBehaviour
{
    //all the nodes that can be travled to
    public List<NodeMarker> nextNode;
    public bool isOccupied = false;
    public string objTag;

    //visualize all of the nodes connected
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, 0.25f);

        foreach(var node in nextNode) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(this.transform.position, node.gameObject.transform.position);
            }
        }

    private void OnTriggerEnter(Collider other) {
        if (objTag != null && other.gameObject.CompareTag(objTag)) {
            isOccupied = true;
            }
        }
    private void OnTriggerExit(Collider other) {
        if (objTag != null && other.gameObject.CompareTag(objTag)) {
            isOccupied = false;
            }
        }
    }
