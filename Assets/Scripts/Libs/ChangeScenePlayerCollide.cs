using GameStateManagement;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenePlayerCollide : MonoBehaviour
{

    [SerializeField]
    string SceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController controller) && SceneUtility.GetBuildIndexByScenePath(SceneName) >= 0)
        {
            GameStateManager.Instance.SetState(new PlayingState(SceneName));
        }
    }
}
