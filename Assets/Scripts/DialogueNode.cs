using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
public class DialogueNode : Node
{
    public string GUID;
    public string DialogueText;
    public bool EntryPoint = false;
    internal object generatedPort;
    }
