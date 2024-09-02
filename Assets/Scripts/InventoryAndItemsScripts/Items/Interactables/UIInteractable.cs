using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables
{
    //This class should eventually be updated to better match how the rest of the UI is set up
    public class UIInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject UIPrefab;

        private bool isActive = false;
        private GameObject instantiatedUIPrefab;

        public bool DestroyAfterInteracting { get { return false; } }

        public void Interact()
        {
            this.isActive = !this.isActive;

            if (this.isActive)
            {
                //Creates prefab in the center
                this.instantiatedUIPrefab = Instantiate(this.UIPrefab, new Vector3(0, 0, 0), Quaternion.identity);

                //Pause game (should later replace with game's pause system)
                Time.timeScale = 0;
            }
            else
            {
                Destroy(this.instantiatedUIPrefab);

                //UnPause game (should later replace with game's pause system)
                Time.timeScale = 1;
            }
        }

        public bool MeetsCriteriaToInteract(Collider playerCollider)
        {
            return true;
        }
    }
}
