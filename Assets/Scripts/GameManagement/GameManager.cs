using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.RoomSystem;
using GameStateManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameObject CoreSystems;

    public static GameObject escMenu;

    public static GameObject gameCanvas;

    private static GameObject eventSystem;

    private static GameObject inventory;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        Transform manager = Instantiate(new GameObject("GameManager")).GetComponent<Transform>();
        DontDestroyOnLoad(manager);
        manager.gameObject.AddComponent<GameStateManager>();

        CoreSystems = Instantiate(
            Resources.Load<GameObject>("Prefabs/CoreSystems/CoreSystems"),
            manager
        );

        eventSystem = Instantiate(
            Resources.Load<GameObject>("Prefabs/UI/EventSystemPrefab"),
            manager
        );

        gameCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/UI/GameCanvas"), manager);
        gameCanvas.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        gameCanvas.GetComponent<RectTransform>().offsetMin = Vector2.zero;

        manager.gameObject.AddComponent<GameProgressTracker>();

        escMenu = Instantiate(
            Resources.Load<GameObject>("Prefabs/UI/EscMenuPrefab"),
            gameCanvas.transform
        );
        escMenu
            .GetComponent<EscMenuController>()
            .getSettingsMenuPanel()
            .GetComponent<SettingsController>()
            .LoadSettings();

        inventory = Instantiate(
            Resources.Load<GameObject>("Prefabs/UI/Inventory"),
            gameCanvas.transform
        );
    }

    //TODO - Test this with multiple scenes when that is readily available
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        NonRespawnableItemsRemover.Instance.RemoveNonRespawnableItems();
    }
}
