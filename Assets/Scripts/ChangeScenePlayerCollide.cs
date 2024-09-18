using GameStateManagement;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScenePlayerCollide : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController controller))
        {
            GameStateManager.Instance.SetState(new PlayingState("CityBlockout"));
        }
    }
}
