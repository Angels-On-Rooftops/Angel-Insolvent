using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSetup : MonoBehaviour
{
    [SerializeField]
    float minSensitivity, maxSensitivity;

    Resolution[] resolutions;

    //Settings Setups
    public void SetupResolutionDropdown(MonoBehaviour resolutionUIElement)
    {
        var resolutionDropdown = resolutionUIElement as TMPro.TMP_Dropdown;
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption =
                resolutions.ElementAt(i).width
                + " x "
                + resolutions.ElementAt(i).height
                + " @ "
                + resolutions.ElementAt(i).refreshRateRatio;
            resolutionOptions.Add(resolutionOption);
            if (
                resolutions[i].width == Screen.currentResolution.width
                && resolutions[i].height == Screen.currentResolution.height
            )
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(
            delegate
            {
                SetResolution(resolutionDropdown.value);
            }
        );
    }

    public void SetupVolumeSlider(MonoBehaviour volumeUIElement)
    {
        var volumeSlider = volumeUIElement as Slider;
        volumeSlider.onValueChanged.AddListener(
            delegate
            {
                SetVolume(volumeSlider.value);
            }
        );
    }

    public void SetupFullscreenToggle(MonoBehaviour fullscreenUIElement)
    {
        var fullscreenToggle = fullscreenUIElement as Toggle;
        fullscreenToggle.onValueChanged.AddListener(
            delegate
            {
                SetFullscreen(fullscreenToggle.isOn);
            }
        );
    }

    public void SetupLookSensitivitySlider(MonoBehaviour lookSensitivityUIElement)
    {
        var lookSensitivitySlider = lookSensitivityUIElement as Slider;
        lookSensitivitySlider.minValue = minSensitivity;
        lookSensitivitySlider.maxValue = maxSensitivity;
        lookSensitivitySlider.onValueChanged.AddListener(
            delegate
            {
                SetLookSensitivity(lookSensitivitySlider.value);
            }
        );
    }

    public void SetupInputBindButton(MonoBehaviour inputBindButtonUIElement)
    {
        var inputBindButton = inputBindButtonUIElement as Button;
        inputBindButton.onClick.AddListener(
            delegate
            {
                Debug.Log("inputbind button pressed");
            }    
        );
    }

    //Settings Behaviors
    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }

    private void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetLookSensitivity(float lookSensitivity)
    {
        GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<CharacterCamera>()?.SetRotationSensitivity(lookSensitivity);
    }
}