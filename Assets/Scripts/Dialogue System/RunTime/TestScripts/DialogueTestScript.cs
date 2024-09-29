using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTestScript : MonoBehaviour
{
    
    [SerializeField] private DialogueHandler dialogueHandler;
    [SerializeField] private TMP_Text testText;
    
    // Start is called before the first frame update
    void Start()
    {
        testText.text = "The dialogue will update this using a Unity Event method call.";
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
        this.dialogueHandler.NewCharacterName += TestCharacterEvent;
    }

    void OnDisable()
    {
        this.dialogueHandler.NewCharacterName -= TestCharacterEvent;
    }

    void TestCharacterEvent(string newName)
    {
        Debug.Log("New Character name: " + newName);
    }
}
