using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BindingButtonData : MonoBehaviour
{
    [NonSerialized]
    public UnityEngine.UI.Button uiButtonElement;
    [NonSerialized]
    public InputAction action;
    [NonSerialized]
    public bool isKeyboardBind;
}
