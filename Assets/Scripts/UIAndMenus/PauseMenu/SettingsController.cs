using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField]
    public SettingsCategory[] settingsCategories;
    Resolution[] resolutions;

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    UnityEvent doneBehavior;

    Button categoryButtonPrefab;
    GameObject settingsPanelPrefab;
    GameObject singleSettingPrefab;

    GameObject categoriesPanel;
    GameObject settingsPanel;

    Dictionary<Button, GameObject> categoryButtonsDictionary = new Dictionary<Button, GameObject>();

    GameObject activeCategory;

    // Start is called before the first frame update
    void Start()
    {
        categoryButtonPrefab = Resources.Load<Button>("Prefabs/UI/Settings/SettingCategoryButton");
        settingsPanelPrefab = Resources.Load<GameObject>("Prefabs/UI/Settings/VerticalPanelPrefab");
        singleSettingPrefab = Resources.Load<GameObject>("Prefabs/UI/Settings/SingleSettingPrefab");

        categoriesPanel = this.gameObject.transform
            .GetChild(1)
            .gameObject.transform.GetChild(0)
            .gameObject;
        settingsPanel = this.gameObject.transform
            .GetChild(1)
            .gameObject.transform.GetChild(1)
            .gameObject;

        for (int j = 0; j < settingsCategories.Length; j++) //For each category
        {
            //Setup category button
            Button categoryButton = Instantiate(
                categoryButtonPrefab,
                categoriesPanel.gameObject.transform
            );
            categoryButton.onClick.AddListener(
                delegate
                {
                    SwitchSettingsCategory(categoryButton);
                }
            );

            //Setup button text
            string categoryName = settingsCategories[j].name;
            var categoryLabel = categoryButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            categoryLabel.text = categoryName;

            var settingsObjects = settingsCategories[j].settings;

            GameObject newSettingsPanel = Instantiate(
                settingsPanelPrefab,
                settingsPanel.gameObject.transform
            );
            for (int i = 0; i < settingsObjects.Length; i++) //for each setting in category
            {
                GameObject newSetting = Instantiate(
                    singleSettingPrefab,
                    newSettingsPanel.gameObject.transform
                );
                settingsObjects[i].CreateUIElement(newSetting, i);
            }
            newSettingsPanel.SetActive(false);

            categoryButtonsDictionary.Add(categoryButton, newSettingsPanel);
        }

        //Setup done button
        Button doneButton = Instantiate(categoryButtonPrefab, categoriesPanel.gameObject.transform);
        doneButton.onClick.AddListener(
            delegate
            {
                doneBehavior.Invoke();
            }
        );
        var doneLabel = doneButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        doneLabel.text = "Done";

        activeCategory = categoryButtonsDictionary.Values.FirstOrDefault();
        categoryButtonsDictionary.Keys.First().onClick.Invoke();
        categoryButtonsDictionary.Keys.First().Select();
    }

    private void SwitchSettingsCategory(Button categoryButton)
    {
        activeCategory.SetActive(false);
        categoryButtonsDictionary[categoryButton].SetActive(true);
        activeCategory = categoryButtonsDictionary[categoryButton];
    }

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

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    [System.Serializable]
    public class SettingsObject
    {
        public enum UIElementType
        {
            Dropdown,
            Slider,
            Toggle
        };

        [System.Serializable]
        public struct UIElementConfig
        {
            public UIElementType type;
            public string label;

            public UnityEvent<MonoBehaviour> customSetup;
        }

        public UIElementConfig config;
        public GameObject prefab;

        [NonSerialized]
        public object value;

        public void CreateUIElement(GameObject parent, int listIndex)
        {
            //Create setting label
            GameObject settingLabel = new GameObject("Label");
            settingLabel.transform.SetParent(parent.gameObject.transform, false);
            settingLabel.AddComponent<RectTransform>();
            var labelText = settingLabel.AddComponent<TextMeshProUGUI>();
            labelText.text = config.label;

            //Create setting UI element
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

            if (uiElement != null)
            {
                RectTransform newRectTransform = uiElement.GetComponent<RectTransform>();
                Vector2 newPosition = newRectTransform.anchoredPosition;
                newPosition.y -= 50 * listIndex;
                newRectTransform.anchoredPosition = newPosition;
            }
        }

        private void SetupDropdown(TMPro.TMP_Dropdown dropdown)
        {
            if (dropdown.GetType() == typeof(TMPro.TMP_Dropdown))
            {
                dropdown.options.Clear();
                config.customSetup?.Invoke(dropdown);
            }
            else
            {
                Debug.Log("Dropdown prefab is not of type TMPro.TMP_Dropdown on " + config.label);
            }
        }

        private void SetupSlider(Slider slider)
        {
            if (slider.GetType() == typeof(Slider))
            {
                slider.minValue = 0;
                slider.maxValue = 1;
                slider.value = 0.5f;
                config.customSetup?.Invoke(slider);
            }
            else
            {
                Debug.Log("Slider prefab is not of type Slider on " + config.label);
            }
        }

        private void SetupToggle(Toggle toggle)
        {
            if (toggle.GetType() == typeof(Toggle))
            {
                toggle.isOn = true;
                config.customSetup?.Invoke(toggle);
            }
            else
            {
                Debug.Log("Toggle prefab is not of type Toggle on " + config.label);
            }
        }
    }

    [System.Serializable]
    public struct SettingsCategory
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public SettingsObject[] settings;
    }
}
