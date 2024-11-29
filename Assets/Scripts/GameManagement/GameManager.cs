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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        Transform manager = Instantiate(new GameObject("GameManager")).GetComponent<Transform>();
        DontDestroyOnLoad(manager);
        manager.gameObject.AddComponent<GameStateManager>();
        manager.gameObject.AddComponent<GameProgressTracker>();

        CoreSystems = Instantiate(
            Resources.Load<GameObject>("Prefabs/CoreSystems/CoreSystems"),
            manager
        );

        gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
        gameCanvas.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        gameCanvas.GetComponent<RectTransform>().offsetMin = Vector2.zero;

        gameCanvas.GetComponentInChildren<EscMenuController>()
            .getSettingsMenuPanel()
            .GetComponent<SettingsController>()
            .LoadSettings();
    }

    //TODO - Test this with multiple scenes when that is readily available
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        NonRespawnableItemsRemover.Instance.RemoveNonRespawnableItems();
    }
}
