using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableOverlayHelper : MonoBehaviour
{
    public string displayText;

    [SerializeField]
    private Canvas canvas;

    void Start()
    {
        canvas.worldCamera = Camera.main;
        TextMeshProUGUI text = canvas.GetComponentInChildren<TextMeshProUGUI>();
        text.text = displayText;
    }
}
