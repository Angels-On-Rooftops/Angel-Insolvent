using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMarker : MonoBehaviour {
    //all the nodes that can be travled to
    [SerializeField] List<NodeMarker> nextNode; //list of avaliable node
    public List<NodeMarker> NextNode { get { return nextNode; } }
    [SerializeField] string objTag; //objects to check for collison


    //visualize all of the nodes connected
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, 0.25f);

        if (nextNode != null && nextNode.Count > 0) {
            foreach (var node in nextNode) {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(this.transform.position, node.gameObject.transform.position);
                }
            }
        }

    
    }
