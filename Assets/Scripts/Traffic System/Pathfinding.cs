using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //path way from start node to end node
    public List<NodeMarker> path;

    private void Start() {
        path = new List<NodeMarker>();
    }

    /*
     * take random node from neighbor list
     * add to path list if not already in the path
     * check if its destination
     * repeat if not
     */

    public List<NodeMarker> findPath(NodeMarker currentNode, NodeMarker lastNode) {
        NodeMarker tempNode = currentNode.NextNode[Random.Range(0, currentNode.NextNode.Count)];
            
        if (!path.Contains(tempNode)) {
            path.Add(tempNode);
            }
        else if(tempNode == lastNode) {
            path.Add(tempNode);
            }
        
        
        if(tempNode != lastNode) {
            findPath(tempNode, lastNode);
            }
        else if (path.Contains(tempNode)) {
            findPath(currentNode, lastNode);
            }

        return path;
        }


    


}
