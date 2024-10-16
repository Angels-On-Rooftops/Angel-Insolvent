using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static bool launchToMainMenu = false; //CHANGE THIS DEPENDING ON WHETHER YOU WANT
                                                 //TO LAUNCH TO MAIN MENU OR TO YOUR CURRENT SCENE
    public static GameObject escMenu;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        GameObject gameObject = Instantiate(new GameObject("GameManager"));
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<GameStateManager>();
        gameObject.AddComponent<GameProgressTracker>();

        escMenu = Instantiate(Resources.Load("Prefabs/UI/EscMenuPrefab")) as GameObject;
        escMenu.gameObject.transform.parent = gameObject.transform;
    }

    //TODO - Test this with multiple scenes when that is readily available
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        NonRespawnableItemsRemover.Instance.RemoveNonRespawnableItems();
    }
}
