using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items.Interactables
{
    /// <summary>
    /// Attaches the PlayerInteractor to the Player (should only be placed on the Player)
    /// (since having the PlayerInteractor be a MonoBehaviour caused problems)
    /// </summary>
    public class AttachInteractorToPlayer : MonoBehaviour
    {
        /*private PlayerInteractor playerInteractor;

        [SerializeField]
        [Tooltip("The keybinds that control character interaction with NPCs and objects in the environment.")]
        InputAction Interact;

        private void Awake()
        {
            this.playerInteractor = PlayerInteractor.Instance;
        }*/

        /// <summary>
        /// Only use this Property after Awake()
        /// </summary>
        //public PlayerInteractor PlayerInteractor { get { return this.playerInteractor; } }
    }
}
