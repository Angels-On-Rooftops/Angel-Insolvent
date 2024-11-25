using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BindingButtonData : MonoBehaviour
{
    [NonSerialized]
    public Button uiButtonElement;
    [NonSerialized]
    public InputAction action;
}
