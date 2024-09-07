using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableOverlayHelper : MonoBehaviour
{
    public string displayText;
    private Canvas canvas;
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = displayText;
        Debug.Log(text.text);
    }
}
