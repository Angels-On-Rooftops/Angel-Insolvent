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
    public readonly Vector2 defaultNodeSize = new Vector2(150,200);
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

        //dont allow for move or deletaion
        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

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
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        //character dialogue
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt => {
            dialogueNode.DialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);

        //button to add choices
        var button = new Button(() => { AddChoicePort(dialogueNode); });
        button.text = "Add Choice";
        dialogueNode.titleContainer.Add(button);

        return dialogueNode;
        }

    //adds choices to node
        //output ports are invisible for some reason
    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "") {
        var generatePort = GeneratePort(dialogueNode,Direction.Output);
       
        var oldLabel = generatePort.contentContainer.Q<Label>("type");
        generatePort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount + 1}" : overriddenPortName;

        //character reponse
        var textField = new TextField(){
            name = string.Empty,
            value = choicePortName
            };

        textField.RegisterValueChangedCallback(evt => generatePort.portName = evt.newValue);
        generatePort.contentContainer.Add(new Label("  "));
        generatePort.contentContainer.Add(textField);
        
        //delete unwanted ports
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatePort)){
            text = "X",
            };



        generatePort.contentContainer.Add(deleteButton);
        generatePort.portName = choicePortName;
        dialogueNode.outputContainer.Add(generatePort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        }

    private void RemovePort(DialogueNode dialogueNode, Port generatePort) {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatePort.portName && x.output.node == generatePort.node);
        if (targetEdge.Any()){
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }
        dialogueNode.outputContainer.Remove(generatePort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();

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
