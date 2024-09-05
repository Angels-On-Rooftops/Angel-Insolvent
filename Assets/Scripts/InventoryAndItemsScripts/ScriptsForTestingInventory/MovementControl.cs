using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

namespace Inventory.Testing
{
    public class MovementControl : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private float speed = 5f; 
        private InputAction moveAction; 
 
        public void Initialize(InputAction moveAction) 
        { 
            this.moveAction = moveAction;
            this.moveAction.Enable();
        } 
 
        private void FixedUpdate()
        { 
            float moveDistance = this.speed * Time.deltaTime;
            Vector2 moveDirection = this.moveAction.ReadValue<Vector2>();
            Vector2 moveVector = moveDistance * moveDirection;
 
            float deltaZ = moveVector.y; //Movement forward/backward 
            float deltaX = moveVector.x; //Movement left/right 
 
            this.player.transform.Translate(deltaX, 0, deltaZ);
        }
    }
}

