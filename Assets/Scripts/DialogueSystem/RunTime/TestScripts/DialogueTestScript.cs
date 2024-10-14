using Assets.Scripts.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTestScript : MonoBehaviour
{
    
    [SerializeField] private DialogueHandler dialogueHandler;
    [SerializeField] private TMP_Text testText;

    Maid maid = new Maid();
    
    // Start is called before the first frame update
    void Start()
    {
        //testText.text = "The dialogue will update this using a Unity Event method call.";       
    }

    public void TestEventMethod1()
    {
        testText.text = "This is from the 1st Unity Event method call!";
        Debug.Log(1);
    }

    public void TestEventMethod2()
    {
        testText.text = "This is from the 2nd Unity Event method call!";
        Debug.Log(2);
    }

    void OnEnable()
    {
        //this.dialogueHandler.NewCharacterName += TestCharacterEvent;

        Debug.Log("hi");

        maid.GiveTask(DialogueSystem.BindToEvent("RancidVibes", OnRancidVibes));
        //maid.GiveEvent(DialogueEvents, "RancidVibes", () => Debug.Log("Event Fired"));
    }

    void OnDisable()
    {
        //this.dialogueHandler.NewCharacterName -= TestCharacterEvent;

        //DialogueEvents.RancidVibes -= OnRancidVibes;
        maid.Cleanup();
    }

    void TestCharacterEvent(string newName)
    {
        Debug.Log("New Character name: " + newName);
    }

    void OnRancidVibes()
    {
        Debug.Log("Rancid Vibes Event Fired");
    }
}
