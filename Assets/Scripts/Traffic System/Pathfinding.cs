using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pathfinding : MonoBehaviour
{
    //path way from start node to end node
    [NonSerialized]
    public List<NodeMarker> path = new();

    /*
     * take random node from neighbor list
     * add to path list if not already in the path
     * check if its destination
     * repeat if not
     */

    public List<NodeMarker> FindPath(NodeMarker startNode, NodeMarker endNode)
    {
        NodeMarker tempNode = startNode.nextNode[Random.Range(0, startNode.nextNode.Count)];

        if (!path.Contains(tempNode))
        {
            path.Add(tempNode);
        }
        else if (tempNode == endNode)
        {
            path.Add(tempNode);
        }

        if (tempNode != endNode)
        {
            FindPath(tempNode, endNode);
        }

        return path;
    }
}
