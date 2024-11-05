using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static bool launchToMainMenu = false; //CHANGE THIS DEPENDING ON WHETHER YOU WANT
                                                 //TO LAUNCH TO MAIN MENU OR TO YOUR CURRENT SCENE
    public static GameObject escMenu;

    public static GameObject gameCanvas;

    private static GameObject eventSystem;

    private static GameObject inventory;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        GameObject gameObject = Instantiate(new GameObject("GameManager"));
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<GameStateManager>();

        eventSystem = Instantiate(Resources.Load("Prefabs/UI/EventSystemPrefab")) as GameObject;
        eventSystem.gameObject.transform.SetParent(gameObject.transform);

        gameCanvas = Instantiate(Resources.Load("Prefabs/UI/GameCanvas")) as GameObject;
        gameCanvas.gameObject.transform.SetParent(gameObject.transform);
        // gameCanvas.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        // gameCanvas.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        
        gameObject.AddComponent<GameProgressTracker>();

        escMenu = Instantiate(Resources.Load("Prefabs/UI/EscMenuPrefab")) as GameObject;
        escMenu.gameObject.transform.SetParent(gameCanvas.transform);

        inventory = Instantiate(Resources.Load("Prefabs/UI/NewInventory")) as GameObject;
        inventory.gameObject.transform.SetParent(gameCanvas.transform);
    }

    //TODO - Test this with multiple scenes when that is readily available
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        NonRespawnableItemsRemover.Instance.RemoveNonRespawnableItems();
    }
}
