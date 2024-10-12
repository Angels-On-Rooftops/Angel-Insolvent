using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    // if the game is paused, or the environment should be frozen
    // (i.e. when opening the inventory), set to true
    public static bool isPaused; 

    // Start is called before the first frame update
    void Start()
    {
        isPaused = Time.timeScale < 1f;
    }

    public static void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
