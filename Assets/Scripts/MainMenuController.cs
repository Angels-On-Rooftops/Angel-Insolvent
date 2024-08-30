using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayButton()
    {
        Debug.Log("Pressed Play Button!");
    }

    public void OnSettingsButton()
    {
        Debug.Log("Pressed Settings Button!");
    }

    public void OnQuitButton()
    {
        Debug.Log("Pressed Quit Button!");
        Application.Quit();
    }
}
