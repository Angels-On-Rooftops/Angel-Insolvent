using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphMovement : MonoBehaviour
{
    [SerializeField]
    Pathfinding path; //pathfinding

    [SerializeField]
    MarkGraph map; //all of the nodes
    public List<NodeMarker> pathway; //pathway that car follows
    public NodeMarker node1;
    public NodeMarker node2;
    public float speed = 5.0f;

    public int position = 0;

    // Start is called before the first frame update
    void Start()
    {
        //inital Pathway
        node1 = map.map[Random.Range(0, map.map.Count)];
        node2 = map.map[Random.Range(0, map.map.Count)];

        pathway = path.FindPath(node1, node2);
        this.transform.position = pathway[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        moveCar();
    }

    private void OnDrawGizmos()
    {
        foreach (NodeMarker node in pathway)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(node.transform.position, 0.25f);
        }
        if (pathway.Count >= 2)
        {
            for (int i = 0; i < pathway.Count - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pathway[i].transform.position, pathway[i + 1].transform.position);
            }
        }
    }

    public void moveCar()
    {
        //move the car to current node
        if (this.transform.position != pathway[position].transform.position)
        {
            this.transform.LookAt(pathway[position].transform.position);
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                pathway[position].transform.position,
                speed * Time.deltaTime
            );
        }
        //increment to next node
        else if (position < pathway.Count - 1)
        {
            position++;
        }
        else
        {
            //generate new pathway after end is reached
            position = 0;
            resetPathway();
        }
    }

    //function to get new pathway once destination is reached
    //new path always needs to start with the old end and get new end
    public void resetPathway()
    {
        pathway.Clear(); //erase old pathway
        node1 = node2; //swith end to be start
        node2 = map.map[Random.Range(0, map.map.Count)]; //new start
        pathway = path.FindPath(node1, node2); //calculate path for new start and end
    }

    //stop cars from hitting each other
    //make one of the cars generate a new pathway || stop one of the cars until the other is out of range
}
