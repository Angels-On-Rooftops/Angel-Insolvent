using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private DialogueContainer dialogueContainer;
    [SerializeField] private TMP_Text currentDialogueText;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Vector2 firstButtonPosition;
    [SerializeField] private Vector2 buttonDisplacement;
    [SerializeField] private GameObject parentCanvas;

    //private NodeLinkData currentNodeLinkData;
    private DialogueNodeData currentDialogueNodeData;

    private List<GameObject> currentButtons = new List<GameObject>();

    void Awake()
    {
        //Get the first node link, which has the PortName "Next"
        NodeLinkData firstNodeLink = this.dialogueContainer.NodeLinks.First(node => node.PortName == "Next");
            
        //Get the first dialogue node by following where the link goes
        this.currentDialogueNodeData = this.dialogueContainer.DialogueNodeData.First(node => node.NodeGuID == firstNodeLink.TargetNodeGuID);

        getCurrentScreen();
    }

    private void getCurrentScreen()
    {
        //Clear previous buttons
        for (int i = this.currentButtons.Count - 1; i >= 0; i--)
        {
            Destroy(this.currentButtons[i]);
        }
            
        this.currentDialogueText.text = this.currentDialogueNodeData.DialogueText;

        List<NodeLinkData> childNodes = 
            this.dialogueContainer.NodeLinks.Where(node => node.BaseNodeGuID == this.currentDialogueNodeData.NodeGuID).ToList();

        Vector2 buttonPosition = this.firstButtonPosition;
        foreach (NodeLinkData childNode in childNodes)
        {
            CreateButton(buttonPosition, childNode);

            buttonPosition += this.buttonDisplacement;
        }
    }

    private void CreateButton(Vector2 buttonPosition, NodeLinkData childNode)
    {
        GameObject newButton = Instantiate(this.buttonPrefab, buttonPosition, Quaternion.identity, this.parentCanvas.transform);
            
        Button buttonComponent = newButton.GetComponentInChildren<Button>();
        buttonComponent.onClick.AddListener(delegate {OnButtonPress(newButton);});

        newButton.GetComponentInChildren<TMP_Text>().text = childNode.PortName;

        this.currentButtons.Add(newButton);
    }

    void OnButtonPress(GameObject button)
    {
        //Update current node with node button port goes to
        string buttonPortName = button.GetComponentInChildren<TMP_Text>().text;
        NodeLinkData nextNodeLinkData = this.dialogueContainer.NodeLinks.First(node => node.PortName == buttonPortName);
        this.currentDialogueNodeData = this.dialogueContainer.DialogueNodeData.First(node => node.NodeGuID == nextNodeLinkData.TargetNodeGuID);

        getCurrentScreen();
    }
}
