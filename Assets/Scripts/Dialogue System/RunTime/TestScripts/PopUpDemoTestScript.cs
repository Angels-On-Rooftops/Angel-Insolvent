using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDemoTestScript : MonoBehaviour
{
    [SerializeField] private GameObject popUpUIPrefab;
    [SerializeField] private float timeBeforeCallPopUp;
    [SerializeField] private bool repeatPopUp;

    private float timePassed = 0;
    private bool mayCreatePopUp = true;
    private GameObject instantiatedPopUp;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.timePassed += Time.deltaTime;

        if (this.timePassed >= this.timeBeforeCallPopUp && this.mayCreatePopUp == true)
        {
            this.mayCreatePopUp = false;

            this.instantiatedPopUp = Instantiate(this.popUpUIPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            DialogueHandler dialogueHandler = this.instantiatedPopUp.GetComponentInChildren<DialogueHandler>();
            dialogueHandler.EndDialogueNodeReached += DestroyDialogueInteractable;

            this.timePassed = 0;
        }
    }

    void DestroyDialogueInteractable()
    {
        Destroy(this.instantiatedPopUp);

        if (this.repeatPopUp)
        {
            this.mayCreatePopUp = true;
        }
    }
}
