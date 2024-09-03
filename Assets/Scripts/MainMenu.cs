using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void OnPlayButton()
    {
        // SceneManager.LoadScene(scene_number)
        Debug.Log("Hit Play Button");
    }

    public void OnSettingsButton()
    {
        // Load settings screen
        Debug.Log("Hit Settings Button");
    }
    public void OnQuitButton()
    {
        Application.Quit();
        Debug.Log("Hit Quit Button");
    }
}
