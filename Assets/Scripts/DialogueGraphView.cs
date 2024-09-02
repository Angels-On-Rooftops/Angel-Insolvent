using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150,200);
    public DialogueGraphView() {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        AddElement(GenerateEntryPointNode());
        }

    //add start node to the graph when opening graph
    private DialogueNode GenerateEntryPointNode() {
        var node = new DialogueNode {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "EntryPoint",
            EntryPoint = true
            };

        //generates output connector for start node
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        //places the first node
        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
        }

    //allows familial node relationships
    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single) {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    
        
        }

    //creates new node
    public DialogueNode CreateDialogueNode(string nodeName) {
        var dialogueNode = new DialogueNode {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
            };

        //adds input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        //button to choices
        var button = new Button(() => { AddChoicePort(dialogueNode); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
        }

    //adds choices to node
    private void AddChoicePort(DialogueNode dialogueNode) {
        var generatePort = GeneratePort(dialogueNode,Direction.Output);
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var outputPortName = $"Choice {outputPortCount}";

        dialogueNode.outputContainer.Add(generatePort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        }

    //adds node to graph
    public void CreateNode(string nodeName) {
        AddElement(CreateDialogueNode(nodeName));
        }

    //lets ports be connected
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port => {
            //don't connect a port to itself
            if(startPort != port && startPort.node != port.node) {
                compatiblePorts.Add(port);
                }
        });

        return compatiblePorts;
        }

    
    }
