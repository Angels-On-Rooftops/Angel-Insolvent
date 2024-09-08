using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

namespace Items.Interactables.Dialogue
{
    public class DialogueHandler : MonoBehaviour
    {
        [SerializeField] private DialogueContainer dialogueContainer;
        [SerializeField] private TMP_Text currentDialogueText;
        [SerializeField] private Button button1;
        [SerializeField] private Button button2;

        //private NodeLinkData currentNodeLinkData;
        private DialogueNodeData currentDialogueNodeData;

        void Awake()
        {
            //Get the first node link, which has the PortName "Next"
            NodeLinkData firstNodeLink = this.dialogueContainer.NodeLinks.First(node => node.PortName == "Next");
            
            //Get the first dialogue node by following where the link goes
            this.currentDialogueNodeData = this.dialogueContainer.DialogueNodeData.First(node => node.NodeGuID == firstNodeLink.TargetNodeGuID);

            this.currentDialogueText.text = this.currentDialogueNodeData.DialogueText;



            /*List<NodeLinkData> childNodes = this.dialogueContainer.NodeLinks.Where(x => x.BaseNodeGuID == Nodes[i].GUID).ToList();
            this.button1.GetComponent<TMP_Text>().text = this.currentDialogueNodeData.DialogueText;*/
        }

        private void getText()
        {

        }
    }
}
