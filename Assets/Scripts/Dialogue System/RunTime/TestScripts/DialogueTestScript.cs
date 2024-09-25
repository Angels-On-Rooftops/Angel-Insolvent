using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTestScript : MonoBehaviour
{
    
    [SerializeField] private DialogueHandler dialogueHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintNum1()
    {
        Debug.Log(1);
    }

    public void PrintNum2()
    {
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
