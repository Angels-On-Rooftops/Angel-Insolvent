using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Interactables
{
    public interface IInteractable
    {
        bool MeetsCriteriaToInteract(Collider playerCollider);

        void Interact();

        bool DestroyAfterInteracting { get; }

        void EnableInteractableCanvas();

        void DisableInteractableCanvas();
    }
}
