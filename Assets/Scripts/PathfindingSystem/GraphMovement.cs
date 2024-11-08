using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIDI2EventSystem;
using System;
using Utility;
using Random = UnityEngine.Random;
public class GraphMovement : MonoBehaviour {
    [SerializeField] Pathfinding path; //pathfinding
    [SerializeField] MarkGraph map; //all of the nodes
    public float speed; //speed of object
    public float Speed { set { speed = value; } get { return speed; } }

    private List<NodeMarker> pathway; //pathway that objeect follows
    private int position = 0; //position in pathway
    private NodeMarker node1; //start node
    private NodeMarker node2; //end node


    [SerializeField] MIDI2EventUnity handler;
    [SerializeField] int BeatPeriod;
    



    void Awake() {
        //inital Pathway
        node1 = map.map[Random.Range(0, map.map.Count)];
        node2 = map.map[Random.Range(0, map.map.Count)];

        pathway = path.findPath(node1, node2);
        this.transform.position = pathway[0].transform.position;

        }

    // Update is called once per frame
    void Update() {
        if (!handler.IsPlaying) {
            return;
            }
        speed = handler.BeatPerSec * 1 / BeatPeriod * Time.deltaTime;
        moveCar();
        }


    public void moveCar() {
        if (this.transform.position != pathway[position].transform.position) {
            this.transform.LookAt(pathway[position].transform.position);
            this.transform.position = Vector3.MoveTowards(this.transform.position, pathway[position].transform.position, speed);
            }
        else {
            changeGoal();
            }
        }

    public void changeGoal() {
        if (this.transform.position == pathway[position].transform.position) {
            //increment to next point
            if (position < pathway.Count - 1) {
                position++;

                }
            else {
                //generate new pathway after end is reached
                position = 0;
                resetPathway();
                }
            }

        }
    public void resetPathway() {
        pathway.Clear(); //erase old pathway
        node1 = node2; //swith end to be start
        node2 = map.map[Random.Range(0, map.map.Count)]; //new end
        pathway = path.findPath(node1, node2); //calculate path for new start and end
        }
    }
