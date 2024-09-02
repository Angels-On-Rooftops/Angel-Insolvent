using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow() {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent(text: "Dialogue Graph");
        }


    //open the editing window
    private void OnEnable() {
        ConstructGraphView();
        GenerateToolbar();
        }


    private void ConstructGraphView() {
        _graphView = new DialogueGraphView {
            name = "Dialogue Graph"
            };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
        }

    //close the editing window
    private void OnDisable() {
        rootVisualElement.Remove(_graphView);
        _graphView = new DialogueGraphView {
            name = "Dialogue Graph"
            };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
        }

    //make toolbar for different actions
    private void GenerateToolbar() {
        var toolbar = new Toolbar();

        //new node button
        var nodeCreateButton = new Button(clickEvent: () => { _graphView.CreateNode("Dialogue Node"); });
        nodeCreateButton.text = "Create Node";
        toolbar.Add(nodeCreateButton);


        rootVisualElement.Add(toolbar);
        }
    }
