using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIDI2EventSystem;
using System;
using Utility;
using Random = UnityEngine.Random;

public class moveObject : MonoBehaviour
{
    [SerializeField] Pathfinding path; //pathfinding
    [SerializeField] MarkGraph map; //all of the nodes
    public List<NodeMarker> pathway; //pathway that car follows
    public NodeMarker node1;
    public NodeMarker node2;
    public float speed = 5.0f;

    public int position = 0; //position in pathway

    [SerializeField]
    MIDI2EventUnity handler;

    [SerializeField]
    Notes note;

    [SerializeField]
    int octave;
    Action unsub;



    // Start is called before the first frame update
    void Awake() {
        //inital Pathway
        node1 = map.map[Random.Range(0, map.map.Count)];
        node2 = map.map[Random.Range(0, map.map.Count)];

        pathway = path.findPath(node1, node2);
        this.transform.position = pathway[0].transform.position;

        }

    // Update is called once per frame
    void Update()
    {
        moveCar();
    }
    /*
    private void OnEnable() {
        unsub = handler.Subscribe(changeGoal, note, octave);
        }

    private void OnDisable() {
        unsub.Invoke();
        }
    */

    public void moveCar() {
        if (this.transform.position != pathway[position].transform.position) {
            this.transform.LookAt(pathway[position].transform.position);
            this.transform.position = Vector3.MoveTowards(this.transform.position, pathway[position].transform.position, speed * Time.deltaTime);
            }
        else {
            changeGoal();
            }
        }

   public void changeGoal() {
        if (this.transform.position == pathway[position].transform.position) {
            //increment to next point
            if (position < pathway.Count - 1) {
                if (!pathway[position + 1].isOccupied) {
                    position++;
                    }
                
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
        node2 = map.map[Random.Range(0, map.map.Count)]; //new start
        pathway = path.findPath(node1, node2); //calculate path for new start and end
        }
    }
