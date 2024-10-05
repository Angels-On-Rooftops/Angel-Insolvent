using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MarkGraph : MonoBehaviour
{
    //all of the nodes
    public List<NodeMarker> map;

    //node to start populating list
    [SerializeField]
    private NodeMarker firstNode;

    // Start is called before the first frame update
    void Start()
    {
        map.Clear();
        map.Add(firstNode);
        PopulateMap(firstNode);
    }

    // Update is called once per frame
    void Update()
    {
        //PopulateMap(firstNode);
    }

    //add every node in graph to list
    private void PopulateMap(NodeMarker startingNode)
    {
        foreach (NodeMarker node in startingNode.nextNode)
        {
            if (!map.Contains(node))
            {
                map.Add(node);
                PopulateMap(node);
            }
        }
    }
}
