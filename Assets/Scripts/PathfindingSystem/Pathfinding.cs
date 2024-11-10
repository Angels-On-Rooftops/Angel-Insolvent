using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
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
        int replaceNode = 0; ;
        path.Add(tempNode);
   
        if (tempNode != lastNode) {
            findPath(tempNode, lastNode);
            } 

        return path;
        }
    /*set order
     * each node has one option
     * 
     */
    public List<NodeMarker> travelPath(NodeMarker currentNode, NodeMarker lastNode) {



        return path;
        }




    }
