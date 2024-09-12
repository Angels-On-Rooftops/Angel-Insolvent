using GameStateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //Eventually remove this check--Saran added it to make running individual scenes temporarily easier
    //Whoever is reviewing this pull request, if I forget to put this back to true, please leave a comment
    private static bool runGameManager = false;
    
    public static GameObject escMenu;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void OnBeforeSplashScreen()
    {
        if (runGameManager)
        {
            GameObject gameObject = Instantiate(new GameObject("GameManager"));
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<GameStateManager>();
            escMenu = Instantiate(Resources.Load("Prefabs/UI/EscMenuPrefab")) as GameObject;
            escMenu.gameObject.transform.parent = gameObject.transform;
        } 
    }
}
