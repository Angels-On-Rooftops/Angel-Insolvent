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

        styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

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
    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "", string defaultPortName = null) {
        var generatePort = GeneratePort(dialogueNode,Direction.Output);
       
        var oldLabel = generatePort.contentContainer.Q<Label>("type");
        generatePort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        string choicePortName;
        if (defaultPortName == null)
        {
            choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount}" : overriddenPortName;
        }
        else
        {
            choicePortName = string.IsNullOrEmpty(overriddenPortName) ? defaultPortName : overriddenPortName;
        }

        //character reponse
        var textField = new TextField {
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
        
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
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

    /*** Additional Types of Nodes ***/

    //Creates new Checker Dialogue node
    public DialogueNode CreateCheckerDialogueNode()
    {
        var dialogueNode = new DialogueNode
        {
            title = DialogueConstants.CheckerNodeName,
            DialogueText = DialogueConstants.CheckerNodeName,
            GUID = Guid.NewGuid().ToString()
        };

        //adds input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        //buttons to add ports
        var passButton = new Button(() => { AddChoicePort(dialogueNode, "Pass: [Insert Checker Index]"); });
        passButton.text = "Add Pass Check";
        dialogueNode.titleContainer.Add(passButton);
        var failButton = new Button(() => { AddChoicePort(dialogueNode, "Fail: [Insert Checker Index]"); });
        failButton.text = "Add Fail Check";
        dialogueNode.titleContainer.Add(failButton);

        return dialogueNode;
    }

    //adds node to graph
    public void CreateCheckerNode()
    {
        AddElement(CreateCheckerDialogueNode());
    }

    //Creates new Event node
    public DialogueNode CreateEventTriggerNode()
    {
        var dialogueNode = new DialogueNode
        {
            title = DialogueConstants.EventNodeName,
            DialogueText = DialogueConstants.EventNodeName,
            GUID = Guid.NewGuid().ToString()
        };

        //adds input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        //button to add event indices
        var button = new Button(() => { AddChoicePort(dialogueNode, "[Insert Event Index]"); });
        button.text = "Add Event";
        dialogueNode.titleContainer.Add(button);

        return dialogueNode;
    }

    //adds node to graph
    public void CreateEventNode()
    {
        AddElement(CreateEventTriggerNode());
    }

    //Creates new Character node
    public DialogueNode CreateNewCharacterSpeakingNode()
    {
        var dialogueNode = new DialogueNode
        {
            title = DialogueConstants.CharacterNodeName,
            DialogueText = DialogueConstants.CharacterNodeName,
            GUID = Guid.NewGuid().ToString()
        };

        //adds input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        AddChoicePort(dialogueNode, "[Insert Character speaking next]");

        return dialogueNode;
    }

    //adds node to graph
    public void CreateCharacterNode()
    {
        AddElement(CreateNewCharacterSpeakingNode());
    }

    //Creates new Timer node (node that moves to the next node after a given amount of time)
    public DialogueNode CreateNewMoveOnTimerNode(string nodeName)
    {
        var dialogueNode = new DialogueNode
        {
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

        AddChoicePort(dialogueNode, "Time (sec): [Time before moving to next node in sec]");

        return dialogueNode;
    }

    //adds node to graph
    public void CreateTimerNode(string nodeName)
    {
        AddElement(CreateNewMoveOnTimerNode(nodeName));
    }

    //Creates new End node
    public DialogueNode CreateEndHereNode()
    {
        var dialogueNode = new DialogueNode
        {
            title = DialogueConstants.EndNodeName,
            DialogueText = DialogueConstants.EndNodeName,
            GUID = Guid.NewGuid().ToString()
        };

        //adds input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }

    //adds node to graph
    public void CreateEndNode()
    {
        AddElement(CreateEndHereNode());
    }
}
