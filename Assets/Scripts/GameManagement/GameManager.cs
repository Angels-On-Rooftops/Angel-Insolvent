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

    public static GameObject gameCanvas;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSplashScreen()
    {
        GameObject gameObject = Instantiate(new GameObject("GameManager"));
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<GameStateManager>();

        gameCanvas = Instantiate(Resources.Load("Prefabs/UI/GameCanvas")) as GameObject;
        gameCanvas.gameObject.transform.parent = gameObject.transform;

        escMenu = Instantiate(Resources.Load("Prefabs/UI/EscMenuPrefab")) as GameObject;
        escMenu.gameObject.transform.parent = gameCanvas.transform;
    }
}
