using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static bool launchToMainMenu = true; //CHANGE THIS DEPENDING ON WHETHER YOU WANT
                                                 //TO LAUNCH TO MAIN MENU OR TO YOUR CURRENT SCENE
    public static GameObject escMenu;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSplashScreen()
    {
        GameObject gameObject = Instantiate(new GameObject("GameManager"));
        DontDestroyOnLoad(gameObject);
        gameObject.AddComponent<GameStateManager>();
        escMenu = Instantiate(Resources.Load("Prefabs/UI/EscMenuPrefab")) as GameObject;
        escMenu.gameObject.transform.parent = gameObject.transform;
    }
}
