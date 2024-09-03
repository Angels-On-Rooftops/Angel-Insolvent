using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace Inventory.Testing
{ 
    public class InputManager : MonoBehaviour 
    { 
        [SerializeField] private MovementControl movementController; 
        private InputActions inputScheme;
        
        private void Awake() 
        { 
            inputScheme = new InputActions(); 
            movementController.Initialize(inputScheme.Player.Move); 
        } 
    } 
} 
