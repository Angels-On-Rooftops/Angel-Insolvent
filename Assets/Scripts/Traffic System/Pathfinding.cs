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

    // TODO does not work, will not find a path if the random node chosen doesn't have some path to the endNode not through the start node
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
