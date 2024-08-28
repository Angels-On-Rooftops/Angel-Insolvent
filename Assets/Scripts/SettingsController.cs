using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public SettingsObject[] settingsObjects;
    float currVolume;
    Resolution[] resolutions;
    public GameObject settingsTitle;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < settingsObjects.Length; i++)
        {
            settingsObjects[i].CreateUIElement(settingsTitle, i);
        }
        //resolutionDropdown.ClearOptions();
        //List<string> resolutionOptions = new List<string>();
        //resolutions = Screen.resolutions;
        //int currentResolutionIndex = 0;
        //for(int i = 0; i < resolutions.Length; i++)
        //{
        //    string resolutionOption = resolutions[i].width + " x " + resolutions[i].height;
        //    resolutionOptions.Add(resolutionOption);
        //    if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
        //    {
        //        currentResolutionIndex = i;
        //    }
        //}
        //resolutionDropdown.AddOptions(resolutionOptions);
        //resolutionDropdown.RefreshShownValue();
        //LoadSettings(currentResolutionIndex);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(float vol)
    {
        //audioSource.volume = vol;
        currVolume = vol;
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SaveSettings()
    {
        //PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetFloat("VolumePreference", currVolume);
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        //if (PlayerPrefs.HasKey("ResolutionPreference"))
        //{
        //    resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        //}
        //else
        //{
        //    resolutionDropdown.value = currentResolutionIndex;
        //}

        //volumeSlider.value = PlayerPrefs.GetFloat("VolumePreference");
    }

    [System.Serializable]
    public class SettingsObject
    {
        public enum UIElementType { Dropdown, Slider, Toggle };

        [System.Serializable]
        public struct UIElementConfig
        {
            public UIElementType type;
            public string label;

            public Action<TMPro.TMP_Dropdown> dropdownSetup;
            public Action<Slider> sliderSetup;
            public Action<Toggle> toggleSetup;
        }

        public UIElementConfig config;
        public GameObject prefab;

        public void CreateUIElement(GameObject parent, int listIndex)
        {
            GameObject uiElement = null;

            uiElement = Instantiate(prefab, parent.gameObject.transform);

            switch (config.type)
            {
                case UIElementType.Dropdown:
                    SetupDropdown(uiElement.GetComponent<TMPro.TMP_Dropdown>());
                    break;
                case UIElementType.Slider:
                    SetupSlider(uiElement.GetComponent<Slider>());
                    break;
                case UIElementType.Toggle:
                    SetupToggle(uiElement.GetComponent<Toggle>());
                    break;
            }

            if(uiElement != null)
            {
                RectTransform newRectTransform = uiElement.GetComponent<RectTransform>();
                Vector2 newPosition = newRectTransform.anchoredPosition;
                newPosition.y -= 50 * listIndex;
                newRectTransform.anchoredPosition = newPosition;
            }
        }

        private void SetupDropdown(TMPro.TMP_Dropdown dropdown)
        {
            dropdown.options.Clear();
            config.dropdownSetup?.Invoke(dropdown);
        }

        private void SetupSlider(Slider slider)
        {
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 50;
            config.sliderSetup?.Invoke(slider);
        }

        private void SetupToggle(Toggle toggle)
        {
            toggle.isOn = true;
            config.toggleSetup?.Invoke(toggle);
        }
    }
}
