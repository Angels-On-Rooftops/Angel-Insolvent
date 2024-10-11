using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableOverlayHelper : MonoBehaviour
{
    public string actionText;
    public string objectNameText;
    public string buttonText;

    [SerializeField]
    private Canvas canvas;

    void Start()
    {
        canvas.worldCamera = Camera.main;
        TextMeshProUGUI action = canvas.transform.Find("ActionPopup").GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI objectName = canvas.transform.Find("ObjectPopup").GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI button = canvas.transform.Find("ButtonPopup").GetComponentInChildren<TextMeshProUGUI>();
        action.text = actionText;
        objectName.text = objectNameText;
        button.text = buttonText;
        // TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
        // text.text = actionText;
    }

    public void EnableCanvas()
    {
        this.canvas.gameObject.SetActive(true);
    }

    public void DisableCanvas()
    {
        this.canvas.gameObject.SetActive(false);
    }
}
