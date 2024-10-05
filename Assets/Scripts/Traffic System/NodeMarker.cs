using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMarker : MonoBehaviour
{
    //all the nodes that can be travled to
    public List<NodeMarker> nextNode;

    //visualize all of the nodes connected
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.25f);

        foreach (var node in nextNode)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, node.gameObject.transform.position);
        }
    }
}
