using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Interactables;
using UnityEngine.UI;

public class Test2 : MonoBehaviour
{    
    [SerializeField] private Button interactButton;

    // Start is called before the first frame update
    void Start()
    {
        this.interactButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        PlayerInteractor.Instance.OnInInteractionRadius += ActivateInteractButton;
        PlayerInteractor.Instance.OnLeaveInteractionRadius += DeActivateInteractButton;
        PlayerInteractor.Instance.CanInteractUpdate += UpdateInteractButton;
    }

    void OnDisable()
    {
        PlayerInteractor.Instance.OnInInteractionRadius -= ActivateInteractButton;
        PlayerInteractor.Instance.OnLeaveInteractionRadius -= DeActivateInteractButton;
        PlayerInteractor.Instance.CanInteractUpdate -= UpdateInteractButton;
    }

    void ActivateInteractButton(bool mayInteract)
    {
        this.interactButton.gameObject.SetActive(true);

        this.interactButton.interactable = mayInteract;
    }

    void DeActivateInteractButton()
    {
        this.interactButton.gameObject.SetActive(false);
    }

    void UpdateInteractButton(bool mayInteract)
    {
        this.interactButton.interactable = mayInteract;
    }

    public void Interact()
    {
        PlayerInteractor.Instance.Interact();
    }
}
