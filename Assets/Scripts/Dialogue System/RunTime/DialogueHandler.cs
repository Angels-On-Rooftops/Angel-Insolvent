using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private DialogueContainer dialogueContainer;
    [SerializeField] private TMP_Text currentDialogueText;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Vector2 firstButtonPosition;
    [SerializeField] private Vector2 buttonDisplacement;
    [SerializeField] private GameObject parentCanvas;

    [SerializeField]
    [Tooltip("The keybinds that allow the player to move forward in the dialogue (when not choosing a specific response), as well as to select an answer")]
    InputAction InteractAction;

    [Tooltip("Text should have the first character's name")]
    [SerializeField] private TMP_Text currentCharacterSpeakingNameText;

    [Tooltip("GameObjects with Component that implements IChecker (Index order must match indices in Graph)")]
    [SerializeField] private GameObject[] checkers;

    [SerializeField] private UnityEvent[] events;

    /// <summary>
    /// Event is triggered when a new character node is reached.
    /// The string inside the port of the node (which is assumed to be the new character's name) is passed in.
    /// </summary>
    public event Action<string> NewCharacterName;

    public event Action EndDialogueNodeReached;

    private const int checkerSubStrEndIndex = 6;
    private const string checkerSubStrPass = "Pass: ";
    private const string checkerSubStrFail = "Fail: ";
    private const int timerSubStrEndIndex = 12;
    private const string timerSubStr = "Time (sec): ";

    //private NodeLinkData currentNodeLinkData;
    private DialogueNodeData currentDialogueNodeData;

    private List<GameObject> currentButtons = new List<GameObject>();

    private string moveForwardToPortID = null;
    private bool moveForwardOnInput = false;
    private float timeToStayOnCurrentNode;
    private bool useTimer = false;
    private float timeEnteredCurrentNode;

    private void OnEnable()
    {
        InteractAction.performed += OnMoveForward;
        InteractAction.Enable();
    }

    private void OnDisable()
    {
        InteractAction.performed -= OnMoveForward;
        InteractAction.Disable();
    }

    void Awake()
    {
        //Get the first node link, which has the PortName "Next"
        NodeLinkData firstNodeLink = this.dialogueContainer.NodeLinks.First(node => node.PortName == "Next");
            
        //Get the first dialogue node by following where the link goes
        this.currentDialogueNodeData = this.dialogueContainer.DialogueNodeData.First(node => node.NodeGuID == firstNodeLink.TargetNodeGuID);

        HandleSpecialNodes();
        GetCurrentScreen();
    }

    private void Update()
    {
        if (!this.useTimer)
        {
            return;
        }

        float timeSpentOnCurrentNode = Time.unscaledTime - this.timeEnteredCurrentNode;
        //Debug.Log("timeSpentOnCurrentNode: " + timeSpentOnCurrentNode);

        if (timeSpentOnCurrentNode >= this.timeToStayOnCurrentNode)
        {
            //Update the current node to the one to move forward to (the next one)
            this.currentDialogueNodeData = this.dialogueContainer.DialogueNodeData.FirstOrDefault(node => node.NodeGuID == this.moveForwardToPortID);

            this.useTimer = false;
            this.moveForwardToPortID = null;

            HandleSpecialNodes();
            GetCurrentScreen();
        }
    }

    private void GetCurrentScreen()
    {
        //Clear previous buttons
        for (int i = this.currentButtons.Count - 1; i >= 0; i--)
        {
            Destroy(this.currentButtons[i]);
        }
        this.currentButtons.Clear();
            
        this.currentDialogueText.text = this.currentDialogueNodeData.DialogueText;

        List<NodeLinkData> childNodes = GetChildrenNodeLinkData();

        Vector2 buttonPosition = this.firstButtonPosition;
        foreach (NodeLinkData childNode in childNodes)
        {
            if (!CheckIfShouldLetPlayerMoveForwardWithoutMakingAChoice(childNode)
                    && !CheckIfShouldEndAfterTime(childNode))
            {
                CreateButton(buttonPosition, childNode);

                buttonPosition += this.buttonDisplacement;
            } 
        }

        if (this.currentButtons.Count > 0)
        {
            Button firstButton = this.currentButtons[0].GetComponentInChildren<Button>();
            firstButton.Select();
        }
    }

    private bool CheckIfShouldLetPlayerMoveForwardWithoutMakingAChoice(NodeLinkData childNode)
    {
        if (childNode.PortName == DialogueConstants.MoveForwardOnInputString)
        {
            this.moveForwardToPortID = childNode.TargetNodeGuID;
            this.moveForwardOnInput = true;

            return true;
        }
        
        return false;
    }

    private bool CheckIfShouldEndAfterTime(NodeLinkData childNode)
    {
        if (childNode.PortName.Length <= timerSubStrEndIndex)
        {
            return false;
        }

        string substr = childNode.PortName.Substring(0, timerSubStrEndIndex);
        if (substr != timerSubStr)
        {
            return false;
        }

        string timeSubstr = childNode.PortName.Substring(timerSubStrEndIndex);
        if (!float.TryParse(timeSubstr, out float time))
        {
            Debug.LogError("Timer Node Port Name format should be \"" + timerSubStr + "[Time before moving to next node in sec]\"");
        }

        this.timeToStayOnCurrentNode = time;
        this.timeEnteredCurrentNode = Time.unscaledTime; //time in seconds since the start of the game

        this.moveForwardToPortID = childNode.TargetNodeGuID;
        this.useTimer = true;

        return true;
    }

    private List<NodeLinkData> GetChildrenNodeLinkData()
    {
        return this.dialogueContainer.NodeLinks.Where(node => node.BaseNodeGuID == this.currentDialogueNodeData.NodeGuID).ToList();
    }

    private void CreateButton(Vector2 buttonPosition, NodeLinkData childNode)
    {
        GameObject newButton = Instantiate(this.buttonPrefab, buttonPosition, Quaternion.identity, this.parentCanvas.transform);
            
        Button buttonComponent = newButton.GetComponentInChildren<Button>();
        buttonComponent.onClick.AddListener(delegate {OnButtonPress(newButton);});

        newButton.GetComponentInChildren<TMP_Text>().text = childNode.PortName;

        this.currentButtons.Add(newButton);

        SetLastButtonNavigation();
    }

    private void SetLastButtonNavigation()
    {
        int index = this.currentButtons.Count - 1;

        if (index <= 0)
        {
            return;
        }

        Button lastButton = this.currentButtons[index].GetComponentInChildren<Button>();
        Button prevButton = this.currentButtons[index - 1].GetComponentInChildren<Button>();

        Navigation lastButtonNav = new Navigation();
        lastButtonNav.mode = Navigation.Mode.Explicit;

        Navigation prevButtonNav;
        if (index == 1)
        {
            prevButtonNav = new Navigation();
            prevButtonNav.mode = Navigation.Mode.Explicit;
        }
        else
        {
            prevButtonNav = prevButton.navigation;
        }

        lastButtonNav.selectOnLeft = prevButton;
        prevButtonNav.selectOnRight = lastButton;

        lastButtonNav.selectOnUp = prevButton;
        prevButtonNav.selectOnDown = lastButton;

        lastButton.navigation = lastButtonNav;
        prevButton.navigation = prevButtonNav;
    }

    void OnButtonPress(GameObject button)
    {
        StartCoroutine(MoveToNextScreenAfterDelay(button));
    }

    IEnumerator MoveToNextScreenAfterDelay(GameObject button)
    {
        //After change pause, change to: yield return new WaitForSeconds(.5f);
        yield return new WaitForSecondsRealtime(.5f);

        //Update current node with node button port goes to
        string buttonPortName = button.GetComponentInChildren<TMP_Text>().text;
        UpdateCurrentNode(buttonPortName);

        GetCurrentScreen();
    }

    void OnMoveForward(CallbackContext c)
    {
        //if should moveForwardOnInput and
        //if after a full performance of the action (ex. both a press and release of a button)
        //to handle the case where the same input was used to trigger the creation 
        //as to move forward and the first node is a node that should moveForwardOnInput
        //(ex. NPC dialogue where the first node does not require the player make a choice)
        if (this.moveForwardOnInput && c.performed)
        {
            //Update the current node to the one to move forward to (the next one)
            this.currentDialogueNodeData = this.dialogueContainer.DialogueNodeData.FirstOrDefault(node => node.NodeGuID == this.moveForwardToPortID);

            this.moveForwardOnInput = false;
            this.moveForwardToPortID = null;

            HandleSpecialNodes();
            GetCurrentScreen();
        }
    }

    /// <summary>
    /// returns false if the node was not updated
    /// </summary>
    private bool UpdateCurrentNode(string nextPortName)
    {
        NodeLinkData nextNodeLinkData = this.dialogueContainer.NodeLinks.FirstOrDefault(node => node.PortName == nextPortName);
        if (nextNodeLinkData == null)
        {
            return false;
        }
        this.currentDialogueNodeData = this.dialogueContainer.DialogueNodeData.FirstOrDefault(node => node.NodeGuID == nextNodeLinkData.TargetNodeGuID);
        if (this.currentDialogueNodeData == null)
        {
            return false;
        }

        HandleSpecialNodes();
        return true;
    }

    /// <summary>
    /// Determine whether the current node is a special node, and if it is,
    /// handle accordingly until the current node is not a special node
    /// </summary>
    private void HandleSpecialNodes()
    {
        if (this.currentDialogueNodeData.DialogueText == DialogueConstants.CheckerNodeName)
        {
            HandleCheckerNode();
        }
        else if (this.currentDialogueNodeData.DialogueText == DialogueConstants.EventNodeName)
        {
            HandleEventNode();
        }
        else if (this.currentDialogueNodeData.DialogueText == DialogueConstants.CharacterNodeName)
        {
            HandleCharacterNode();
        }
        else if (this.currentDialogueNodeData.DialogueText == DialogueConstants.EndNodeName)
        {
            HandleEndNode();
        }
        // else, do nothing (this.currentDialogueNodeData is a regular dialogue node)
    }

    /// <summary>
    /// Go down path of the first port where the checker indicated by the index
    /// meets criteria if the port is a pass port or does not meet criteria if it is a fail port
    /// </summary>
    private void HandleCheckerNode()
    {
        List<NodeLinkData> childNodes = GetChildrenNodeLinkData();
        foreach (NodeLinkData childNode in childNodes)
        {
            string indexSubstr = childNode.PortName.Substring(checkerSubStrEndIndex);
            if (!Int32.TryParse(indexSubstr, out int index))
            {
                Debug.LogError("Checker Node Port Name format should be either \"" + checkerSubStrPass + "[Checker Index]\" or \"" + checkerSubStrFail + "[Checker Index]\"");
                continue;
            }
            IChecker checker = this.checkers[index].GetComponent<IChecker>();
            
            string substr = childNode.PortName.Substring(0, checkerSubStrEndIndex);
            if ( (substr == checkerSubStrPass && checker.MeetsCriteria()) ||
                    (substr == checkerSubStrFail && !checker.MeetsCriteria()) )
            {
                UpdateCurrentNode(childNode.PortName);
            }
        }
    }

    /// <summary>
    /// Invoke each event that matches the index of a port of this Node;
    /// then go down the path of the first port that is connected to another Node
    /// </summary>
    private void HandleEventNode()
    {
        List<NodeLinkData> childNodes = GetChildrenNodeLinkData();
        foreach (NodeLinkData childNode in childNodes)
        {
            if (!Int32.TryParse(childNode.PortName, out int index))
            {
                Debug.LogError("Event Node Port Name format should be [Event Index]");
                continue;
            }

            this.events[index]?.Invoke();
        }

        foreach (NodeLinkData childNode in childNodes)
        {
            bool updated = UpdateCurrentNode(childNode.PortName);
            if (updated)
            {
                return;
            }
        }
    }

    private void HandleCharacterNode()
    {
        List<NodeLinkData> childNodes = GetChildrenNodeLinkData();
        if (childNodes.Count != 1)
        {
            Debug.LogError("Character Node should only have one port");
        }
        string newCharacterSpeakingName = childNodes[0].PortName;

        this.currentCharacterSpeakingNameText.text = newCharacterSpeakingName;

        if (NewCharacterName != null)
        {
            NewCharacterName(newCharacterSpeakingName);
        }

        UpdateCurrentNode(newCharacterSpeakingName);
    }

    private void HandleEndNode()
    {
        if (EndDialogueNodeReached != null)
        {
            EndDialogueNodeReached();
        }

        //Debug.Log("Reached end of Dialogue");

        //Destroy(this.gameObject);
    }
}
