using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


//lets graphees be saved and loaded into editor
public class GraphSaveUtility 
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();


    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView) {
        return new GraphSaveUtility{
            _targetGraphView = targetGraphView
            }; 
        }
    public void SaveGraph(string fileName) {
        //dont need to save empty graph
        if(!Edges.Any())
                return;
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray(); //only count connected edges
        for(int i = 0; i < connectedPorts.Length; i++) {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData {
                BaseNodeGuID = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuID = inputNode.GUID
                }) ;
            }
        foreach(var dialgoueNode in Nodes.Where(node => !node.EntryPoint)) { //dont need start node
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData{
                NodeGuID = dialgoueNode.GUID,
                DialogueText = dialgoueNode.DialogueText,
                position = dialgoueNode.GetPosition().position
                });
            }

        //create folder if necessary
        if (!AssetDatabase.IsValidFolder("Assets/Resources")) {
            AssetDatabase.CreateFolder("Assets", "Resources");
            }
        //save graph
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
        

        }
    public void LoadGraph(string fileName) {
        _containerCache = Resources.Load<DialogueContainer>(fileName);
        if (_containerCache == null) {
            EditorUtility.DisplayDialog("File not found", "Target dialogue graph does not exist", "OK");
            return;
            }
        ClearGraph();
        CreateNodes();
        ConnectNodes();
        }

    private void ConnectNodes() {
        for(int i = 0; i < Nodes.Count; i++) {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuID == Nodes[i].GUID).ToList();
            for (int j = 0; j < connections.Count; j++) {
                var targetNodeGuid = connections[j].TargetNodeGuID;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                targetNode.SetPosition(new Rect(_containerCache.DialogueNodeData.First(x => x.NodeGuID == targetNodeGuid).position, 
                    _targetGraphView.defaultNodeSize));
                }
            }
        }

    private void LinkNodes(Port port1, Port port2) {
        var tempEdge = new Edge{ 
            output = port1,
            input = port2
            };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);

        }



    private void CreateNodes() {
        foreach(var nodeData in _containerCache.DialogueNodeData) {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText);
            tempNode.GUID = nodeData.NodeGuID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuID == nodeData.NodeGuID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
            }
        }

    private void ClearGraph() {
        Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuID;
        foreach(var node in Nodes) {
            if (node.EntryPoint) continue;
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
            _targetGraphView.RemoveElement(node);
            }

        }
    }
