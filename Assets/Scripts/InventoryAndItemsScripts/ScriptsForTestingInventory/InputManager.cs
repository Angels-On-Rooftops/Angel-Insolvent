using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace Inventory.Testing
{ 
    public class InputManager : MonoBehaviour 
    { 
        [SerializeField] private MovementControl movementController; 
        [SerializeField] private EscMenuController escMenuController; 
        private InputActions inputScheme;
        private UITestHandler uiTestHandler;
        
        private void Awake() 
        { 
            inputScheme = new InputActions(); 
            movementController.Initialize(inputScheme.Player.Move);
            uiTestHandler = new UITestHandler(inputScheme.Player.Menu, escMenuController);
        }
    } 
} 
